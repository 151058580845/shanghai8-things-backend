using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线消息数据库迁移类
    /// 重要数据持久化存储
    /// </summary>
    public class MqttOfflineDatabaseMigration
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<MqttOfflineDatabaseMigration> _logger;

        public MqttOfflineDatabaseMigration(ISqlSugarClient sqlSugarClient, ILogger<MqttOfflineDatabaseMigration> logger)
        {
            _sqlSugarClient = sqlSugarClient;
            _logger = logger;
        }

        /// <summary>
        /// 执行数据库迁移
        /// </summary>
        /// <returns></returns>
        public async Task MigrateAsync()
        {
            try
            {
                _logger.LogInformation("开始执行MQTT离线消息数据库迁移...");

                // 1. 创建主表
                await CreateMainTableAsync();

                // 2. 创建索引
                await CreateIndexesAsync();

                // 3. 创建视图
                await CreateViewsAsync();

                // 4. 创建存储过程
                await CreateStoredProceduresAsync();

                // 5. 创建触发器
                await CreateTriggersAsync();

                _logger.LogInformation("MQTT离线消息数据库迁移完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT离线消息数据库迁移失败");
                throw;
            }
        }

        /// <summary>
        /// 创建主表
        /// </summary>
        /// <returns></returns>
        private async Task CreateMainTableAsync()
        {
            try
            {
                // 使用SqlSugar的CodeFirst功能创建表
                _sqlSugarClient.CodeFirst.InitTables(typeof(MqttOfflineMessageEntity));
                _logger.LogDebug("创建MQTT离线消息表完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建MQTT离线消息表失败");
                throw;
            }
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        private async Task CreateIndexesAsync()
        {
            try
            {
                var indexes = new[]
                {
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_equip_id ON mqtt_offline_messages(equip_id)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_created_time ON mqtt_offline_messages(created_time)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_is_sent ON mqtt_offline_messages(is_sent)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_priority ON mqtt_offline_messages(priority)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_retry_count ON mqtt_offline_messages(retry_count)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_sent_time ON mqtt_offline_messages(sent_time)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_message_type ON mqtt_offline_messages(message_type)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_business_id ON mqtt_offline_messages(business_id)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_equip_priority ON mqtt_offline_messages(equip_id, priority)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_equip_sent ON mqtt_offline_messages(equip_id, is_sent)",
                    "CREATE INDEX IF NOT EXISTS idx_mqtt_offline_created_sent ON mqtt_offline_messages(created_time, is_sent)"
                };

                foreach (var indexSql in indexes)
                {
                    await _sqlSugarClient.Ado.ExecuteCommandAsync(indexSql);
                }

                _logger.LogDebug("创建索引完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建索引失败");
                throw;
            }
        }

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <returns></returns>
        private async Task CreateViewsAsync()
        {
            try
            {
                // 创建消息统计视图
                var statsViewSql = @"
                    CREATE OR REPLACE VIEW v_mqtt_offline_stats AS
                    SELECT 
                        COUNT(*) as total_messages,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count < max_retry_count THEN 1 ELSE 0 END) as pending_messages,
                        SUM(CASE WHEN is_sent = TRUE THEN 1 ELSE 0 END) as sent_messages,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count >= max_retry_count THEN 1 ELSE 0 END) as failed_messages,
                        SUM(payload_size) as total_size_bytes,
                        MIN(created_time) as oldest_message_time,
                        MAX(created_time) as newest_message_time,
                        AVG(payload_size) as average_message_size,
                        COUNT(DISTINCT equip_id) as unique_equips
                    FROM mqtt_offline_messages";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(statsViewSql);

                // 创建按设备统计的视图
                var equipStatsViewSql = @"
                    CREATE OR REPLACE VIEW v_mqtt_offline_stats_by_equip AS
                    SELECT 
                        equip_id,
                        COUNT(*) as total_messages,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count < max_retry_count THEN 1 ELSE 0 END) as pending_messages,
                        SUM(CASE WHEN is_sent = TRUE THEN 1 ELSE 0 END) as sent_messages,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count >= max_retry_count THEN 1 ELSE 0 END) as failed_messages,
                        SUM(payload_size) as total_size_bytes,
                        MIN(created_time) as oldest_message_time,
                        MAX(created_time) as newest_message_time,
                        AVG(payload_size) as average_message_size
                    FROM mqtt_offline_messages
                    WHERE equip_id IS NOT NULL
                    GROUP BY equip_id";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(equipStatsViewSql);

                // 创建按优先级统计的视图
                var priorityStatsViewSql = @"
                    CREATE OR REPLACE VIEW v_mqtt_offline_stats_by_priority AS
                    SELECT 
                        priority,
                        COUNT(*) as message_count,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count < max_retry_count THEN 1 ELSE 0 END) as pending_count,
                        SUM(CASE WHEN is_sent = TRUE THEN 1 ELSE 0 END) as sent_count,
                        SUM(CASE WHEN is_sent = FALSE AND retry_count >= max_retry_count THEN 1 ELSE 0 END) as failed_count
                    FROM mqtt_offline_messages
                    GROUP BY priority
                    ORDER BY priority";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(priorityStatsViewSql);

                _logger.LogDebug("创建视图完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建视图失败");
                throw;
            }
        }

        /// <summary>
        /// 创建存储过程
        /// </summary>
        /// <returns></returns>
        private async Task CreateStoredProceduresAsync()
        {
            try
            {
                // 创建清理已发送消息的存储过程
                var cleanupProcedureSql = @"
                    CREATE PROCEDURE IF NOT EXISTS sp_cleanup_sent_messages(
                        IN days_to_keep INT DEFAULT 7
                    )
                    BEGIN
                        DECLARE deleted_count INT DEFAULT 0;
                        
                        DELETE FROM mqtt_offline_messages 
                        WHERE is_sent = TRUE 
                        AND sent_time < DATE_SUB(NOW(), INTERVAL days_to_keep DAY);
                        
                        SET deleted_count = ROW_COUNT();
                        
                        SELECT deleted_count as deleted_messages;
                    END";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(cleanupProcedureSql);

                // 创建获取待发送消息的存储过程
                var getPendingMessagesProcedureSql = @"
                    CREATE PROCEDURE IF NOT EXISTS sp_get_pending_messages(
                        IN p_equip_id CHAR(36),
                        IN p_max_count INT DEFAULT 100
                    )
                    BEGIN
                        SELECT 
                            id, equip_id, topic, payload, payload_size, created_time,
                            retry_count, max_retry_count, priority, is_sent, sent_time,
                            last_retry_time, error_message, message_type, business_id, extended_properties
                        FROM mqtt_offline_messages
                        WHERE is_sent = FALSE 
                        AND retry_count < max_retry_count
                        AND (p_equip_id IS NULL OR equip_id = p_equip_id)
                        ORDER BY priority ASC, created_time ASC
                        LIMIT p_max_count;
                    END";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(getPendingMessagesProcedureSql);

                // 创建批量标记消息为已发送的存储过程
                var markMessagesSentProcedureSql = @"
                    CREATE PROCEDURE IF NOT EXISTS sp_mark_messages_sent(
                        IN p_message_ids TEXT
                    )
                    BEGIN
                        DECLARE sql_stmt TEXT;
                        
                        SET sql_stmt = CONCAT(
                            'UPDATE mqtt_offline_messages SET is_sent = TRUE, sent_time = NOW() WHERE id IN (', 
                            p_message_ids, 
                            ')'
                        );
                        
                        SET @sql = sql_stmt;
                        PREPARE stmt FROM @sql;
                        EXECUTE stmt;
                        DEALLOCATE PREPARE stmt;
                        
                        SELECT ROW_COUNT() as updated_messages;
                    END";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(markMessagesSentProcedureSql);

                _logger.LogDebug("创建存储过程完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建存储过程失败");
                throw;
            }
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <returns></returns>
        private async Task CreateTriggersAsync()
        {
            try
            {
                // 创建插入时自动计算payload_size的触发器
                var insertTriggerSql = @"
                    CREATE TRIGGER IF NOT EXISTS tr_update_payload_size 
                    BEFORE INSERT ON mqtt_offline_messages
                    FOR EACH ROW
                    BEGIN
                        SET NEW.payload_size = LENGTH(NEW.payload) * 3 / 4;
                    END";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(insertTriggerSql);

                // 创建更新时自动计算payload_size的触发器
                var updateTriggerSql = @"
                    CREATE TRIGGER IF NOT EXISTS tr_update_payload_size_update 
                    BEFORE UPDATE ON mqtt_offline_messages
                    FOR EACH ROW
                    BEGIN
                        IF NEW.payload != OLD.payload THEN
                            SET NEW.payload_size = LENGTH(NEW.payload) * 3 / 4;
                        END IF;
                    END";

                await _sqlSugarClient.Ado.ExecuteCommandAsync(updateTriggerSql);

                _logger.LogDebug("创建触发器完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建触发器失败");
                throw;
            }
        }

        /// <summary>
        /// 检查数据库是否已初始化
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsInitializedAsync()
        {
            try
            {
                // 检查表是否存在
                var tableExists = await _sqlSugarClient.Ado.GetIntAsync(
                    "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'mqtt_offline_messages'") > 0;

                if (!tableExists)
                    return false;

                // 检查视图是否存在
                var viewExists = await _sqlSugarClient.Ado.GetIntAsync(
                    "SELECT COUNT(*) FROM information_schema.views WHERE table_name = 'v_mqtt_offline_stats'") > 0;

                return viewExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查数据库初始化状态失败");
                return false;
            }
        }

        /// <summary>
        /// 回滚迁移（删除所有相关对象）
        /// </summary>
        /// <returns></returns>
        public async Task RollbackAsync()
        {
            try
            {
                _logger.LogWarning("开始回滚MQTT离线消息数据库迁移...");

                var rollbackCommands = new[]
                {
                    "DROP TRIGGER IF EXISTS tr_update_payload_size_update",
                    "DROP TRIGGER IF EXISTS tr_update_payload_size",
                    "DROP PROCEDURE IF EXISTS sp_mark_messages_sent",
                    "DROP PROCEDURE IF EXISTS sp_get_pending_messages",
                    "DROP PROCEDURE IF EXISTS sp_cleanup_sent_messages",
                    "DROP VIEW IF EXISTS v_mqtt_offline_stats_by_priority",
                    "DROP VIEW IF EXISTS v_mqtt_offline_stats_by_equip",
                    "DROP VIEW IF EXISTS v_mqtt_offline_stats",
                    "DROP TABLE IF EXISTS mqtt_offline_messages"
                };

                foreach (var command in rollbackCommands)
                {
                    await _sqlSugarClient.Ado.ExecuteCommandAsync(command);
                }

                _logger.LogWarning("MQTT离线消息数据库迁移回滚完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT离线消息数据库迁移回滚失败");
                throw;
            }
        }

        /// <summary>
        /// 验证数据库结构
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ValidateDatabaseStructureAsync()
        {
            try
            {
                // 检查表结构
                var tableColumns = await _sqlSugarClient.Ado.SqlQueryAsync<dynamic>(
                    "SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_NAME = 'mqtt_offline_messages'");

                var requiredColumns = new[]
                {
                    "id", "equip_id", "topic", "payload", "payload_size", "created_time",
                    "retry_count", "max_retry_count", "priority", "is_sent", "sent_time",
                    "last_retry_time", "error_message", "message_type", "business_id", "extended_properties"
                };

                var existingColumns = tableColumns.Select(c => c.COLUMN_NAME.ToString()).ToHashSet();

                foreach (var column in requiredColumns)
                {
                    if (!existingColumns.Contains(column))
                    {
                        _logger.LogError($"缺少必需的列: {column}");
                        return false;
                    }
                }

                // 检查索引
                var indexes = await _sqlSugarClient.Ado.SqlQueryAsync<dynamic>(
                    "SELECT INDEX_NAME FROM information_schema.STATISTICS WHERE TABLE_NAME = 'mqtt_offline_messages'");

                var requiredIndexes = new[]
                {
                    "idx_mqtt_offline_equip_id", "idx_mqtt_offline_created_time", "idx_mqtt_offline_is_sent",
                    "idx_mqtt_offline_priority", "idx_mqtt_offline_retry_count", "idx_mqtt_offline_sent_time"
                };

                var existingIndexes = indexes.Select(i => i.INDEX_NAME.ToString()).ToHashSet();

                foreach (var index in requiredIndexes)
                {
                    if (!existingIndexes.Contains(index))
                    {
                        _logger.LogWarning($"缺少索引: {index}");
                    }
                }

                _logger.LogInformation("数据库结构验证完成");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证数据库结构失败");
                return false;
            }
        }
    }
}
