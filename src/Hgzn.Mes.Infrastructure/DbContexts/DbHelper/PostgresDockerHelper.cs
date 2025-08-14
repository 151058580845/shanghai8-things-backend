using System;
using System.Diagnostics;
using System.IO;

public class PostgresDockerHelper
{
    private readonly string _containerName;
    private readonly string _username;
    private readonly string _password;
    private readonly string _backupFolder;

    public PostgresDockerHelper(string containerName, string username, string password, string backupFolder)
    {
        _containerName = containerName;
        _username = username;
        _password = password;
        _backupFolder = backupFolder;
    }

    /// <summary>
    /// 自动备份数据库到指定文件夹
    /// </summary>
    public string Backup(string database)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupFile = Path.Combine(_backupFolder,"Postgres", $"{database}_{timestamp}.backup");

        // docker exec pg_dump 命令
        var psi = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"exec -i {_containerName} pg_dump -U {_username} -F c {database}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.EnvironmentVariables["PGPASSWORD"] = _password;

        using (var process = Process.Start(psi))
        {
            using (var fs = new FileStream(backupFile, FileMode.Create))
            {
                process.StandardOutput.BaseStream.CopyTo(fs);
            }

            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"PostgreSQL备份失败: {error}");
            }
        }

        Console.WriteLine($"数据库备份成功: {backupFile}");
        return backupFile;
    }

    /// <summary>
    /// 从备份文件恢复数据库
    /// </summary>
    public void Restore(string database, string backupFile)
    {
        // 检查备份文件是否存在
        if (!File.Exists(backupFile))
            throw new FileNotFoundException("备份文件不存在", backupFile);

        var psi = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"exec -i {_containerName} pg_restore -U {_username} -d {database} -c",
            RedirectStandardInput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.EnvironmentVariables["PGPASSWORD"] = _password;

        using (var process = Process.Start(psi))
        {
            using (var fs = new FileStream(backupFile, FileMode.Open))
            {
                fs.CopyTo(process.StandardInput.BaseStream);
            }

            process.StandardInput.Close();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"PostgreSQL恢复失败: {error}");
            }
        }

        Console.WriteLine($"数据库恢复成功: {database}");
    }

    /// <summary>
    /// 自动备份并可选立即恢复到另一个数据库
    /// </summary>
    public void BackupAndRestore(string sourceDatabase, string targetDatabase = null)
    {
        string backupFile = Backup(sourceDatabase);

        if (!string.IsNullOrEmpty(targetDatabase))
        {
            Restore(targetDatabase, backupFile);
        }
    }
}
