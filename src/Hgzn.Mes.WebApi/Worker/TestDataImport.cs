
using Hgzn.Mes.Application.Main.Services.Equip.IService;

namespace Hgzn.Mes.WebApi.Worker
{
    public class TestDataImport : BackgroundService
    {
        private ITestDataService _testDataService;
        private ILogger<TestDataImport> _logger;
        public TestDataImport(ITestDataService testDataService, ILogger<TestDataImport> logger)
        {
            _testDataService = testDataService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1 * 5 * 1000);
            while (true)
            {
                var ret = await _testDataService.GetDataFromThirdPartyAsync();
                _logger.LogInformation($"尝试获取试验计划数据,更新数量{ret}");
                await Task.Delay(60 * 60 * 1000); // 一个小时获取一次
            }
        }
    }
}
