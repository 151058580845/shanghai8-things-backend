using MQTTnet.Client;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager
{
    public interface IMqttExplorer
    {
        public TopicBuilder WillTopicBuilder { get;set; }

        /// <summary>
        /// 启动连接
        /// </summary>
        /// <param name="server">服务器Ip</param>
        /// <param name="port">端口</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task StartAsync(string server, int port, string username, string password);
        /// <summary>
        /// 使用默认参数启动连接
        /// </summary>
        Task StartAsync();
        /// <summary>
        /// 停止连接
        /// </summary>
        /// <returns></returns>
        Task StopAsync();

        /// <summary>
        /// 发送一个主题数据
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="payload">消息内容</param>
        /// <returns></returns>
        Task PublishAsync(string topic, byte[] payload);
        /// <summary>
        /// 发送一个主题数据
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="payload">消息内容</param>
        /// <returns></returns>
        Task PublishAsync(string topic, string payload);
        /// <summary>
        /// 取消订阅一个主题
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task UnSubscribeAsync(string topic);
        /// <summary>
        /// 订阅一个主题
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task SubscribeAsync(string topic);
        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="server">服务器Ip</param>
        /// <param name="port">端口</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task RestartAsync(string server, int port, string username, string password);
        /// <summary>
        /// 获取当前的连接状态
        /// </summary>
        /// <returns></returns>
        Task<bool> IsConnectedAsync();
        
        /// <summary>
        /// 连接回调
        /// </summary>
        event Func<MqttClientConnectedEventArgs,Task>? MqttClientConnected;
        /// <summary>
        /// 断开连接回调
        /// </summary>
        event Func<MqttClientDisconnectedEventArgs,Task> MqttClientDisconnected;
        
        /// <summary>
        /// 获取消息（接收到的消息可以触发事件）
        /// </summary>
        event Func<MqttMessageEventArgs,Task> MessageReceived;
        
        /// <summary>
        /// 获取当前订阅的所有主题
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetSubscribedTopicsAsync();

    }
}