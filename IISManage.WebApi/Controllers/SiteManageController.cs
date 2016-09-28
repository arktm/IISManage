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
        ServerManager server = new ServerManager();
        #region 所有站点列表
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetSiteFullList")]
        public List<SiteModel> GetSiteFullList()
        {
            return server.Sites.OrderBy(x => x.Id).Select(site => site.ToSiteModel()).ToList();
        }

        #endregion
        #region 获取站点信息
        /// <summary>
        /// 得到站点信息
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        [Route("GetSite")]
        public SiteModel GetSite(int siteId=-1,string siteName=null)
        {
            var sites = GetSiteFullList();
            if (siteId > 0)
            {
                return sites.FirstOrDefault(x => x.SiteId == siteId);
            }
            return sites.FirstOrDefault(x => x.SiteName.IsSame(siteName));
        }
        #endregion

        #region 添加域名
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="model"></param>
        /// <param name="moveOrigDomainToThis">强制自动域名到当前站点</param>
        /// <returns></returns>
        [Route("AddBind")]
        public ResultModel PostAddBind(int siteId,[FromBody] BindingModel model,bool moveOrigDomainToThis=false)
        {
            var allSites = GetSiteFullList();
            //是否有，已存在此域名的站点（域名，端口）都相网的站点
            var siteContainDomain = allSites.FirstOrDefault(x => x.Bindings.Any(b => string.Compare(b.Host, model.Host, StringComparison.OrdinalIgnoreCase) == 0 && b.Port==model.Port));
            if (siteContainDomain != null)
            {
                if (!moveOrigDomainToThis)
                {
                   return new ResultModel(false,$"{model.Host}:{model.Port},已绑定于站点,{siteContainDomain.SiteId}::{siteContainDomain.SiteName}");
                }
                else
                {//移除原站点绑定数据
                    DeleteDeleteModel(siteContainDomain.SiteId,
                        siteContainDomain.Bindings.FirstOrDefault(x => x.Host.IsSame(model.Host)));
                }
            }


            var site = server.Sites.FirstOrDefault(x => x.Id == siteId);
            if (site == null)
            {
                return new ResultModel(false, $"站点ID:{siteId},不存在!");
            }
            site.Bindings.Add($"{model.IP}:{model.Port}:{model.Host}", model.Protocol);
            server.CommitChanges();
            server.Dispose();
            return new ResultModel(true,"绑定成功");
        }

        #endregion

        #region 删除域名
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteBind")]
        public ResultModel DeleteDeleteModel(long siteId,[FromBody] BindingModel model)
        {
           var site = server.Sites.FirstOrDefault(x => x.Id == siteId);
            if (site == null)
            {
                return new ResultModel(false,$"站点ID:{siteId},不存在!");
            }
            
            foreach (Binding binding in site.Bindings)
            {
                var bindModel = binding.GetBindModel();
                if (bindModel.Host.IsSame(model.Host) && bindModel.Port==model.Port)
                {
                    site.Bindings.Remove(binding);
                    server.CommitChanges();
                    return new ResultModel(true,"删除成功");
                }
            }
            return new ResultModel(false,$"未找到指定站点下的 域名{model.Host}");
        }
        #endregion


        #region 是否存在
        /// <summary>
        /// 站点是否存在
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        bool IsSiteExists(int siteId)
        {
            return server.Sites.Any(x => x.Id == siteId);
        }
        #endregion
    }


    #region 扩展 

    public static class Helpers
    {
        /// <summary>
        /// 返回 绑定 Model
        /// </summary>
        /// <param name="siteBinding"></param>
        /// <returns></returns>
        public static BindingModel GetBindModel(this Binding siteBinding)
        {
            var values = siteBinding.BindingInformation.Split(new char[] { ':' });
            //ip:port:host
            var b = new BindingModel(values[2], Int32.Parse(values[1]), values[0]) {Protocol = siteBinding.Protocol};
            return b;
        }

        public static bool IsSame(this string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// 转换为SiteModel
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static SiteModel ToSiteModel(this Site site)
        {
            SiteModel s = new SiteModel();
            s.SiteId = site.Id;
            s.SiteName = site.Name;
            s.Bindings = new List<BindingModel>();
            foreach (var b in site.Bindings.Select(binding => binding.GetBindModel()))
            {
                s.Bindings.Add(b);
            }
            return s;
        }
    }
    #endregion
}
