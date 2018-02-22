/****************************************************************************
* Copyright @ 武汉侏罗纪技术开发有限公司 2017. All rights reserved.
* 
* 文 件 名: Extensions
* 创 建 者：zhoush
* 创建日期：2017/7/19 19:44:28
* 功能描述: 
* 
* 修 改 人：    
* 修改时间:     
* 修改日志:    
*
* 审 查 者:     
* 审 查 时 间:  
****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using PKS.Models;
using PKS.WebAPI.Models;
using PKS.WebAPI.ES;

namespace PKS.WebAPI.ES
{
    public static class Extensions<T> where T : class
    {
        //public static SearchResult<T> ToMetadataCollection(ISearchResponse<T> searchResponse)
        //{
        //    return searchResponse.ToMetadataCollection<SearchResult<T>>();
        //}

        public static SearchResult<T> ToMetadataCollection(ISearchResponse<T> searchResponse)           
        {
            SearchResult<T> searchResult = new SearchResult<T>();
            //TargetResult searchResult = new TargetResult();
            searchResult.Metadatas = new MetadataCollection<T>(searchResponse.Documents.AsEnumerable());
            searchResult.Total = Convert.ToInt32(searchResponse.Total);
            return searchResult;
        }
    }
}
