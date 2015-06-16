﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using Mes.Demo.Contracts.SiteManagement;
using Mes.Demo.Dtos.SiteManagement;
using Mes.Demo.Models.SiteManagement;
using Mes.Utility;
using Mes.Utility.Data;
using Mes.Utility.Extensions;
using Mes.Utility.Filter;
using Mes.Web.Mvc.Binders;
using Mes.Web.Mvc.Security;
using Mes.Web.UI;

using Util;

using Enum = Util.Enum;


namespace Mes.Demo.Web.Areas.Admin.Controllers
{
    public class ProblemController : AdminBaseController
    {
        public ISiteManagementContract SiteManagementContract { get; set; }


        [AjaxOnly]
        public ActionResult GridData(int? id)
        {
            int total;
            GridRequest request = new GridRequest(Request);
            var datas = GetQueryData<Problem, int>(SiteManagementContract.Problems, out total, request).Select(m => new
            {
                m.ProblemTime,
                m.Department,
                m.Factory,
                m.QuestionFrom,
                m.Detail,
                m.Reason,
                m.Solution,
                m.IsComplete,
                m.IsPeople,
                m.Workers,
                m.Suggestion,
                m.Remark,
                m.Type,
                m.Id,
                m.CreatedTime
            });
            return Json(new GridData<object>(datas, total), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Add([ModelBinder(typeof(JsonBinder<ProblemDto>))] ICollection<ProblemDto> dtos)
        {
            dtos.CheckNotNull("dtos");
            OperationResult result = SiteManagementContract.AddProblems(dtos.ToArray());
            return Json(result.ToAjaxResult(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Edit([ModelBinder(typeof(JsonBinder<ProblemDto>))] ICollection<ProblemDto> dtos)
        {
            dtos.CheckNotNull("dtos");
            OperationResult result = SiteManagementContract.EditProblems(dtos.ToArray());
            return Json(result.ToAjaxResult(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Delete([ModelBinder(typeof(JsonBinder<int>))] ICollection<int> ids)
        {
            ids.CheckNotNull("ids");
            OperationResult result = SiteManagementContract.DeleteProblems(ids.ToArray());
            return Json(result.ToAjaxResult(), JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        public ActionResult DepartmentReport(string fromDate)
        {
            var data = Data(fromDate, "department");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        public ActionResult WorkersReport(string fromDate)
        {
            var data = Data(fromDate, "workers");
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private object Data(string fromDate,string groupByFiled)
        {
            const string problemTime = "ProblemTime";
            fromDate.CheckNotNullOrEmpty("fromDate");
            DateTime date = Conv.ToDate(fromDate);
            DateTime beginTime;
            DateTime endTime;
            if (date.DayOfWeek != 0)
            {
                beginTime = date.AddDays((double)(1 - date.DayOfWeek));
                endTime = date.AddDays((double)(7 - date.DayOfWeek));
            }
            else
            {
                endTime = date;
                beginTime = date.AddDays(-6);
            }

            var xAxisdata = new ArrayList();
            //部门辅助开关
            var legenddata = new ArrayList();
            //标题
            const string titletext = "异常工时";
            //子标题
            var titlesubtext = beginTime.ToDateString() + "-" + endTime.ToDateString();
            for (int i = 0; i < 7; i++)
            {
                xAxisdata.Add(beginTime.AddDays(i).ToDateString());
            }

            FilterGroup group = new FilterGroup
            {
                Rules = new List<FilterRule>
                {
                    new FilterRule(problemTime, beginTime, FilterOperate.GreaterOrEqual),
                    new FilterRule(problemTime, endTime, FilterOperate.LessOrEqual)
                },
                Operate = FilterOperate.And
            };
            Expression<Func<Problem, bool>> predicate = FilterHelper.GetExpression<Problem>(@group);
            var delayTimes = SiteManagementContract.Problems.Where(predicate).ToList();
            IEnumerable<IGrouping<string, Problem>> dutyGroupList;
            //分组
            if (groupByFiled=="workers")
                dutyGroupList = delayTimes.GroupBy(g => g.Workers);
            else
                dutyGroupList = delayTimes.GroupBy(g => g.Department);

            var seriesList = new ArrayList();
            foreach (var dutyGroup in dutyGroupList)
            {
                legenddata.Add(dutyGroup.Key);
                var dutyhourlist = new ArrayList();

                for (int i = 0; i < 7; i++)
                {
                    int b = dutyGroup.AsQueryable().Count(g => g.ProblemTime.ToDateString() == beginTime.AddDays(i).ToDateString());
                    dutyhourlist.Add(b);
                }
                var markPointData1 = new { type = "max", name = "最大值" };
                var markPointData2 = new { type = "min", name = "最小值" };
                var markPointList = new ArrayList() { markPointData1, markPointData2 };
                var markPoint = new { data = markPointList };

                var markLineData1 = new { type = "average", name = "平均值" };
                var markLineList = new ArrayList() { markLineData1 };
                var markLine = new { data = markLineList };
                var seriestest1 = new { name = dutyGroup.Key, type = "line", data = dutyhourlist, markPoint = markPoint, markLine = markLine };

                seriesList.Add(seriestest1);
            }

            var data = new { seriesList = seriesList, xAxisdata = xAxisdata, legenddata = legenddata, titletext = titletext, titlesubtext = titlesubtext };
            return data;
        }

        public ActionResult Report()
        {
            return View();
        }
	}
}