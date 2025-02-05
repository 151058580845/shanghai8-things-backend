using Newtonsoft.Json;
using StackExchange.Redis;
using Exception = System.Exception;

namespace Hgzn.Mes.Iot.Redis;

public class RedisService
{
    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database= connectionMultiplexer.GetDatabase();
    }

    private readonly IDatabase _database;
    
    public static string GetRedisName(string code)
    {
        return nameof(RedisData) + ":" + code;
    }
    
    public static List<string> GetRedisKeys(IEnumerable<string> codes)
    {
        return codes.Select(GetRedisName).ToList();
    }
    
    /// <summary>
    /// 设置缓存数据
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
    public async Task SetRedisDataAsync(string code, object data)
    {
        RedisData dataDto = new()
        {
            Code = code,
            Date = data
        };
        await _database.SetAddAsync(GetRedisName(code),JsonConvert.SerializeObject(dataDto));
    }
    public async Task SetRedisDataAsync(Guid code, object data)
    {
        await SetRedisDataAsync(code.ToString(), data);
    }
    
    /// <summary>
    /// 根据编号获取对应的缓存数据
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<object?> GetRedisDataAsync(string code)
    {
        try
        {
            var result = await _database.StringGetAsync(GetRedisName(code));
            if (result.HasValue)
            {
                return JsonConvert.DeserializeObject<RedisData>(result.ToString())?.Date;
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    /// <summary>
    /// 批量获取缓存数据
    /// </summary>
    /// <param name="codes">缓存数据的唯一标识符列表</param>
    /// <returns>返回一个字典，键是 Redis 键，值是缓存的数据</returns>
    public async Task<Dictionary<string, object?>> GetRedisDataBatchAsync(IEnumerable<string> codes)
    {
        var result = new Dictionary<string, object?>();
        try
        {
            // 构建 Redis 键的列表
            var redisKeys = codes.Select(t=>(RedisKey)GetRedisName(t)).ToArray();

            var redisValues = await _database.StringGetAsync(redisKeys);
            // 遍历获取到的值并反序列化
            for (int i = 0; i < redisKeys.Length; i++)
            {
                var value = redisValues[i];
                if (!value.IsNullOrEmpty)
                {
                    // 反序列化为 RedisData 对象
                    var dataDto = JsonConvert.DeserializeObject<RedisData>(value!);
                    if (dataDto != null)
                    {
                        result[redisKeys[i]!] = dataDto;  // 保存到结果字典
                    }
                }
                else
                {
                    result[redisKeys[i]!] = null;
                }
            }
        }
        catch (Exception ex)
        {
            // 使用日志框架记录异常
            Console.WriteLine($"Error fetching batch data from Redis: {ex.Message}");
        }

        return result;
    }
    
    /// <summary>
    /// 设置过期时间
    /// </summary>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <param name="options"></param>
    public async Task SetRedisDataOptionsAsync(string code, object data, TimeSpan? options = null)
    {
        var dataDto = new RedisData()
        {
            Code = code,
            Date = data
        };
        // 将对象序列化为 JSON 字符串
        var serializedData = JsonConvert.SerializeObject(dataDto);

        // 设置 Redis 键值并设置过期时间（如果提供了 options）
        var redisKey = GetRedisName(code);

        if (options.HasValue)
        {
            // 设置过期时间
            await _database.StringSetAsync(redisKey, serializedData, options.Value);
        }
        else
        {
            // 如果没有提供过期时间，永久保存
            await _database.StringSetAsync(redisKey, serializedData);
        }
    }
}