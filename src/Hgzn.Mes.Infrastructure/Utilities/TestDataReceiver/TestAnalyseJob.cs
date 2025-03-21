using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class TestAnalyseJob
    {
        public Dictionary<string, string> AbbreviationName = new Dictionary<string, string>()
        {
            { "解析器件","解析" },
            { "垂直精控器件","精垂"},
            { "水平精控器件" , "精水"},
            { "解析控制","解析控制"},
            { "垂直精控控制","精垂控制"},
            { "水平精控控制","精水控制"},
            { "解析精控空用","共用"},
            { "解析精控风扇","风扇"},
            { "垂直粗控","粗垂" },
            { "垂直粗控控制","粗控"},
            { "水平粗控","粗水"},
            { "水平粗控控制","粗控"},
            { "垂直粗控共用供电","共用供电"},
            { "垂直粗控共用控制","共用控制"},
            { "垂直粗控风扇","共用风扇"},
            { "垂直粗控控制共用","共用控制"},
            { "垂直粗控控制风扇","共用风扇"},
            { "水平粗控共用供电","共用供电"},
            { "水平粗控共用控制","共用控制"},
            { "水平粗控风扇","共用风扇"},
            { "水平粗控控制共用","共用控制"},
            { "水平粗控控制风扇","共用风扇"},
        };
        public List<DivisionTable> DivisionTables = new List<DivisionTable>();

        public TestAnalyseJob()
        {
            DivisionTables = InitializeDivisionTables();
        }

        public ApiResponse GetResponse(ReceiveData receiveData, List<string> tableNames)
        {
            ApiResponse ret = new ApiResponse();
            if (receiveData == null) return ret;
            ret.Data = new List<DataArea>();
            // 获取想要查询的表格名
            List<DivisionTable> queryTables = GetTables(tableNames);
            foreach (DivisionTable queryTable in queryTables)
            {
                Dictionary<(string, string), float> analyseData = Analyse(receiveData, queryTable);
                List<Column> columns = GetColumns(queryTable.Title);
                // 初始化数据列表
                List<Dictionary<string, object>> data = BuildData(analyseData, columns);
                // 添加数据区域
                ret.Data.Add(new DataArea
                {
                    Title = queryTable.Title,
                    Columns = columns,
                    Data = data,
                    Span = queryTable.Spans,
                });
            };
            return ret;
        }

        public ApiResponse GetResponseForPushData(ReceiveData receiveData, List<string> tableNames)
        {
            ApiResponse ret = new ApiResponse();
            if (receiveData == null) return ret;
            ret.Data = new List<DataArea>();
            // 获取想要查询的表格名
            List<DivisionTable> queryTables = GetTables(tableNames);
            foreach (DivisionTable queryTable in queryTables)
            {
                Dictionary<(string, string), float> analyseData = Analyse(receiveData, queryTable);
                List<Column> columns = GetColumns(queryTable.Title);
                // 初始化数据列表
                List<Dictionary<string, object>> data = BuildData(analyseData, columns);
                // 添加数据区域
                ret.Data.Add(new DataArea
                {
                    Title = queryTable.Title,
                    Data = data
                });
            };
            return ret;
        }

        private List<DivisionTable> GetTables(List<string> queryTables)
        {
            List<DivisionTable> ret = DivisionTables;
            if (queryTables == null) return ret;
            if (queryTables != null && queryTables.Count > 0)
            {
                ret = new List<DivisionTable>();
                foreach (string item in queryTables)
                {
                    foreach (DivisionTable table in DivisionTables)
                    {
                        if (item == table.Title)
                            ret.Add(table);
                    }
                }
            }
            return ret;
        }

        private List<Dictionary<string, object>> BuildData(Dictionary<(string, string), float> analyseData, List<Column> columns)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            // 填充数据列表
            foreach (KeyValuePair<(string, string), float> item in analyseData)
            {
                string deviceCode = item.Key.Item2;
                string channelName = item.Key.Item1;

                // 查找或创建数据项
                Dictionary<string, object>? dataEntry = data.FirstOrDefault(d => d["DeviceCode"].ToString() == deviceCode);
                if (dataEntry == null)
                {
                    dataEntry = new Dictionary<string, object> { { "DeviceCode", deviceCode } };
                    data.Add(dataEntry);
                }

                // 添加通道数据
                if (channelName == null)
                    dataEntry["合并项里的列写什么呀?"] = item.Value;
                else
                    dataEntry[columns.First(x => x.Name == channelName).Field] = item.Value;
            }
            return data;
        }

        private List<DivisionTable> InitializeDivisionTables()
        {
            return new List<DivisionTable>
            {
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (1, 84)},
                    Title = "解析精控",
                    Spans = new List<Span>
                    {
                        new Span { Row = 11, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 12, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 13, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (85, 114)},
                    Title = "放大器",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (115, 135)},
                    Title = "粗控垂直1区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (136, 156)},
                    Title = "粗控垂直2区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (157, 177)},
                    Title = "粗控垂直3区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (178, 198)},
                    Title = "粗控垂直4区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (199, 219)},
                    Title = "粗控垂直5区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (220, 240)},
                    Title = "粗控垂直6区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (241, 248)},
                    Title = "粗控控制垂直1区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (249, 256)},
                    Title = "粗控控制垂直2区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (257, 264)},
                    Title = "粗控控制垂直3区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (265, 272)},
                    Title = "粗控控制垂直4区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (273, 280)},
                    Title = "粗控控制垂直5区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (281, 288)},
                    Title = "粗控控制垂直6区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (289, 309)},
                    Title = "粗控水平1区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (310, 330)},
                    Title = "粗控水平2区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (331, 351)},
                    Title = "粗控水平3区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (352, 372)},
                    Title = "粗控水平4区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (373, 393)},
                    Title = "粗控水平5区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (394, 414)},
                    Title = "粗控水平6区", Spans = new List<Span>
                    {
                        new Span { Row = 3, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 4, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 5, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (415, 422)},
                    Title = "粗控控制水平1区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (423, 430)},
                    Title = "粗控控制水平2区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (431, 438)},
                    Title = "粗控控制水平3区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (439, 446)},
                    Title = "粗控控制水平4区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (447, 454)},
                    Title = "粗控控制水平5区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                },
                new DivisionTable
                {
                    StartAndEnd = new List<(int, int)>{ (455, 462)},
                    Title = "粗控控制水平6区",
                    Spans = new List<Span>
                    {
                        new Span { Row = 1, Col = 1, Rowspan = 1, Colspan = 6 },
                        new Span { Row = 2, Col = 1, Rowspan = 1, Colspan = 6 }
                    }
                }
            };
        }

        private List<Column> GetColumns(string title)
        {
            return new List<Column>
            {
                new Column { Field = "DeviceCode", Name = title },
                new Column { Field = "Channel1", Name = "通道1" },
                new Column { Field = "Channel2", Name = "通道2" },
                new Column { Field = "Channel3", Name = "通道3" },
                new Column { Field = "Channel4", Name = "通道4" },
                new Column { Field = "Channel5", Name = "通道5" },
                new Column { Field = "Channel6", Name = "通道6" },
            };
        }

        private (string, string) ParseDescription(string description)
        {
            // 使用正则表达式提取通道和数值部分
            Match match = Regex.Match(description, @"(通道\d+)(分区\d+)?(.*?)(\d+V|\-\d+V)");
            if (match.Success)
            {
                string channel = match.Groups[1].Value; // 通道部分
                string deviceType = match.Groups[3].Value; // 设备类型部分
                string voltage = match.Groups[4].Value; // 电压部分

                // 查找设备类型的缩写
                deviceType = GetAbbreviatedDeviceType(deviceType);
                // 返回通道和解析后的设备类型 + 电压
                return (channel, deviceType + voltage);
            }
            match = Regex.Match(description, @"(分区\d+)(.*?)(\d+V|\-\d+V)");
            if (match.Success)
            {
                string zone = null!; // 分区部分这里我故意置为null,用以区分共用和非共用
                string deviceType = match.Groups[2].Value; // 设备类型部分
                string voltage = match.Groups[3].Value; // 电压部分

                // 查找设备类型的缩写
                deviceType = GetAbbreviatedDeviceType(deviceType);
                // 返回通道和解析后的设备类型 + 电压
                return (zone, deviceType + voltage)!;
            }
            return (string.Empty, string.Empty); // 如果没有匹配，返回空
        }

        // 提取设备类型缩写的逻辑
        private string GetAbbreviatedDeviceType(string deviceType)
        {
            foreach (KeyValuePair<string, string> entry in AbbreviationName)
            {
                if (deviceType.Equals(entry.Key))
                {
                    deviceType = deviceType.Replace(entry.Key, entry.Value);
                    break;
                }
            }
            return deviceType;
        }

        private Dictionary<(string, string), float> Analyse(ReceiveData receiveData, DivisionTable divisionTable)
        {
            // 这里的18是指报头占用的属性数量,这样做是为了让divisionTable的start和end就是协议上的"字段号"
            int occupy = 18;
            // 获取 DeviceSettings 类的所有属性
            PropertyInfo[] properties = typeof(ReceiveData).GetProperties();

            // 存储描述信息的字典
            Dictionary<string, float> descriptions = new Dictionary<string, float>();
            // <(通道,数据名称) , 值>
            Dictionary<(string, string), float> abbreviationNameDescriptions = new Dictionary<(string, string), float>();

            foreach ((int, int) startAndEnd in divisionTable.StartAndEnd)
            {
                for (int i = occupy + startAndEnd.Item1; i <= occupy + startAndEnd.Item2; i++)
                {
                    // 获取 Description 特性
                    DescriptionAttribute? descriptionAttribute = properties[i].GetCustomAttribute<DescriptionAttribute>();
                    if (descriptionAttribute != null)
                    {
                        float value = (float)properties[i].GetValue(receiveData);
                        // 将属性名和描述存储到字典中
                        descriptions[descriptionAttribute.Description] = value;
                    }
                }

                foreach (KeyValuePair<string, float> item in descriptions)
                {
                    abbreviationNameDescriptions[ParseDescription(item.Key)] = item.Value;
                }
            }


            return abbreviationNameDescriptions;
        }
    }
}
