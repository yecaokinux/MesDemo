﻿// <autogenerated>
//   This file was generated by T4 code generator Dto.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System;
using System.Linq;
using System.Linq.Expressions;

using Mes.Core;
using Mes.Demo.Dtos.SiteManagement;
using Mes.Demo.Models.SiteManagement;
using Mes.Utility.Data;


namespace Mes.Demo.Contracts.SiteManagement
{
    public partial interface ISiteManagementContract : IDependency
    {
        /// <summary>
        /// 获取异常 信息查询数据集
        /// </summary>
        IQueryable<Problem> Problems { get; }
        
        /// <summary>
        /// 检查异常信息是否存在
        /// </summary>
        /// <param name="predicate">检查谓语表达式</param>
        /// <param name="id">更新的异常信息编号</param>
        /// <returns>异常信息是否存在</returns>
        bool CheckProblemExists(Expression<Func<Problem, bool>> predicate, int id = 0);
        
        /// <summary>
        /// 添加异常信息
        /// </summary>
        /// <param name="dtos">要添加的异常信息DTO信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult AddProblems(params ProblemDto[] dtos);
        
        /// <summary>
        /// 更新异常信息
        /// </summary>
        /// <param name="dtos">包含更新信息的异常DTO信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult EditProblems(params ProblemDto[] dtos);

        /// <summary>
        /// 删除异常信息
        /// </summary>
        /// <param name="ids">要删除的异常信息编号</param>
        /// <returns>业务操作结果</returns>
        OperationResult DeleteProblems(params int[] ids);
    }
}
      