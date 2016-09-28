using System.Collections.Generic;
using System.Net;

namespace IISManage.Models
{
    public class BindingModel
    {
        public BindingModel()
        {
            
        }
        public BindingModel(string domain,int port=80,string ip = "")
        {
            this.Host = domain;
            this.Port = port;
            //this.EndPoint=new IPEndPoint(IPAddress.Parse(ip),port);
            this.IP = ip;
        }
        public string Host { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
        public string Protocol { get; set; } = "http";
    }

    public class SiteModel
    {
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        /// <summary>
        /// 绑定域名
        /// </summary>
        public List<BindingModel> Bindings { get; set; }
    }
}