using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Agent
{
    public class Config
    {
        private static IConfigurationRoot _configuration;

        public static void Load(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public static Config Instance = new Config();

        private Config()
        {
        
        }       

        public string MysqlConnectionString
        {
            get
            {
                return _configuration.GetValue<string>("mysqlConnectionString");
            }
        }
        public string MssqlConnectionString
        {
            get
            {
                return _configuration.GetValue<string>("mssqlConnectionString");
            }
        }

        public string ProjectDownloadUrl
        {
            get
            {
                return _configuration.GetValue<string>("projectDownloadUrl");
            }
        }
        

        public string NodeServerUrl
        {
            get
            {
                return _configuration.GetValue<string>("nodeServerUrl");
            }
        }

    }
}
