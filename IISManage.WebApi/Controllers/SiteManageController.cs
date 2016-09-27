using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IISManage.Models;
using Microsoft.Web.Administration;

namespace IISManage.WebApi.Controllers
{
    [RoutePrefix("v1/SiteManage")]
    public class SiteManageController : ApiController
    {
        #region 所有站点列表
        [Route("GetSiteFullList")]
        public List<SiteModel> GetSiteFullList()
        {
            List<SiteModel> results = new List<SiteModel>();
            ServerManager sm = new ServerManager();
            foreach (Site site in sm.Sites)
            {
                SiteModel s = new SiteModel();
                s.SiteId = site.Id;
                s.SiteName = site.Name;
                s.Bindings = new List<BindingModel>();
                foreach (Binding binding in site.Bindings)
                {
                    var values = binding.BindingInformation.Split(new char[] { ':' });
                    //ip:port:host
                    var b = new BindingModel(values[2], Int32.Parse(values[1]), values[0]);
                    s.Bindings.Add(b);
                }
                results.Add(s);
            }

            return results;
        }
        #endregion

        #region 添加域名
        [Route("AddBind")]
        public ResultModel AddBind(int siteId,[FromBody] BindingModel model)
        {
            var allSites = GetSiteFullList();
            //是否有，已存在此域名的站点（域名，端口）都相网的站点
            var siteContainDomain = allSites.FirstOrDefault(x => x.Bindings.Any(b => string.Compare(b.Host, model.Host, StringComparison.OrdinalIgnoreCase) == 0 && b.Port==model.Port));
            if (siteContainDomain != null)
            {
                return new ResultModel(false,$"{model.Host}:{model.Port},已绑定于站点,{siteContainDomain.SiteId}::{siteContainDomain.SiteName}");
            }
        }

        #endregion
    }
}
