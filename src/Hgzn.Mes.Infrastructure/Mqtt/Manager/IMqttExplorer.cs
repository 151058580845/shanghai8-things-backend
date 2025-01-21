using MQTTnet.Client;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager
{
    public interface IMqttExplorer
    {

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
        Task RestartAsync();

        /// <summary>
        /// 获取当前的连接状态
        /// </summary>
        /// <returns></returns>
        Task<bool> IsConnectedAsync();

    }
}