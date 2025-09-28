using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线消息数据库实体
    /// </summary>
    [Table("mqtt_offline_messages")]
    public class MqttOfflineMessageEntity
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 设备ID
        /// </summary>
        [Column("equip_id")]
        public Guid? EquipId { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [Column("topic")]
        [Required]
        [StringLength(500)]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// 消息内容（Base64编码）
        /// </summary>
        [Column("payload")]
        [Required]
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// 消息大小（字节）
        /// </summary>
        [Column("payload_size")]
        public int PayloadSize { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("created_time")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 重试次数
        /// </summary>
        [Column("retry_count")]
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        [Column("max_retry_count")]
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// 消息优先级（数字越小优先级越高）
        /// </summary>
        [Column("priority")]
        public int Priority { get; set; } = 0;

        /// <summary>
        /// 是否已发送成功
        /// </summary>
        [Column("is_sent")]
        public bool IsSent { get; set; } = false;

        /// <summary>
        /// 发送时间
        /// </summary>
        [Column("sent_time")]
        public DateTime? SentTime { get; set; }

        /// <summary>
        /// 最后重试时间
        /// </summary>
        [Column("last_retry_time")]
        public DateTime? LastRetryTime { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [Column("error_message")]
        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 消息类型（用于分类）
        /// </summary>
        [Column("message_type")]
        [StringLength(100)]
        public string? MessageType { get; set; }

        /// <summary>
        /// 业务ID（用于关联业务数据）
        /// </summary>
        [Column("business_id")]
        [StringLength(200)]
        public string? BusinessId { get; set; }

        /// <summary>
        /// 扩展属性（JSON格式）
        /// </summary>
        [Column("extended_properties")]
        public string? ExtendedProperties { get; set; }

        /// <summary>
        /// 转换为IMqttOfflineMessage
        /// </summary>
        /// <returns></returns>
        public IMqttOfflineMessage ToOfflineMessage()
        {
            return new MqttOfflineMessage
            {
                Id = Id,
                Topic = Topic,
                Payload = Convert.FromBase64String(Payload),
                CreatedTime = CreatedTime,
                RetryCount = RetryCount,
                MaxRetryCount = MaxRetryCount,
                Priority = Priority,
                IsSent = IsSent,
                SentTime = SentTime
            };
        }

        /// <summary>
        /// 从IMqttOfflineMessage创建实体
        /// </summary>
        /// <param name="message">离线消息</param>
        /// <param name="equipId">设备ID</param>
        /// <returns></returns>
        public static MqttOfflineMessageEntity FromOfflineMessage(IMqttOfflineMessage message, Guid? equipId = null)
        {
            return new MqttOfflineMessageEntity
            {
                Id = message.Id,
                EquipId = equipId,
                Topic = message.Topic,
                Payload = Convert.ToBase64String(message.Payload),
                PayloadSize = message.Payload.Length,
                CreatedTime = message.CreatedTime,
                RetryCount = message.RetryCount,
                MaxRetryCount = message.MaxRetryCount,
                Priority = message.Priority,
                IsSent = message.IsSent,
                SentTime = message.SentTime,
                LastRetryTime = message.IsSent ? null : DateTime.UtcNow
            };
        }
    }
}
