﻿using FoodTrace.Common;
using FoodTrace.Common.Libraries;
using FoodTrace.IService;
using FoodTrace.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodTrace.Common.Log;
using Microsoft.Owin.Security.Facebook;

namespace FoodTrace.WebSite.Controllers
{
    public class UserBaseController : BaseController
    {

        IUserBaseService userBaseService;
        IUserRoleService userRoleIndexService;
        IRoleService roleService;
        IUserDetailService userDetailService;
        ICompanyService companyService;
        IDeptService deptService;
        public UserBaseController(IUserRoleService iService,IRoleService rService,IUserDetailService aService, IUserBaseService uService, ICompanyService cService, IDeptService dService)
        {
            userDetailService = aService;
            userBaseService = uService;
            companyService = cService;
            deptService = dService;
            roleService = rService;
            userRoleIndexService = iService;
        }
        // GET: UserBase
        public ActionResult Index()
        {
            var user = new UserBaseDto();
          
            var companylist = companyService.GetPagerCompany(string.Empty, 1, 100).Select(_ => new { CompanyName = _.CompanyName, CompanyID = _.CompanyID });
            ViewBag.CompanyList = new SelectList(companylist, "CompanyID", "CompanyName");
            ViewData.Model = user;
            return View();
        }

        public ActionResult Create()
        {
           // UserManagement.CurrentCompany = new Model.CompanyModel() { CompanyID = 1 };
            var companylist = companyService.GetPagerCompany(string.Empty, 1, 100).Select(_ => new { CompanyName = _.CompanyName, CompanyID = _.CompanyID });
            //var deptlist = deptService.GetPagerDept(string.Empty, 1, 100).Select(_ => new { DeptName = _.DeptName, DeptID = _.DeptID });
            //var rolelist = roleService.GetPagerRole("", 1, 100);
           
            //ViewBag.RoleList = rolelist;
           // ViewBag.DeptList = new SelectList(deptlist, "DeptID","DeptName");
            ViewBag.CompanyList = new SelectList(companylist, "CompanyID", "CompanyName");
            return PartialView();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create(UserDetailModel model)
        {
            bool isSuccess = false;
            try
            {
                userDetailService.InsertSingleUserBase(model);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Json(isSuccess);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var user = userBaseService.GetUserBaseWithDetailById(id);
            var companylist = companyService.GetPagerCompany(string.Empty, 1, 100).Select(_ => new { CompanyName = _.CompanyName, CompanyID = _.CompanyID });
            var deptlist = deptService.GetPagerDept(string.Empty, 1, 100).Select(_ => new { DeptName = _.DeptName, DeptID = _.DeptID });
            //var rolelist = roleService.GetPagerRole("", 1, 100);

            //var userRefRole = userRoleIndexService.GetUserRefRoleByUid(id);
           // ViewBag.UserRefRole = userRefRole;
           // ViewBag.RoleList = rolelist;
            ViewBag.DeptList = new SelectList(deptlist, "DeptID","DeptName",user.DeptID);
            ViewBag.CompanyList = new SelectList(companylist, "CompanyID", "CompanyName",user.CompanyID);
            ViewData.Model = user;
            return View();
        }

        /// <summary>
        /// 获取用户详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetUserById(int id)
        {
            var result = new ResultJson();
            try
            {
                var data = userBaseService.GetUserBaseWithDetailById(id);
                result.IsSuccess = true;
                result.Data = data;
            }
            catch (Exception)
            {

            }

            return JsonEx(result,"yyyy-MM-dd");
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveUserData(UserBaseDto model)
        {
            var result = new ResultJson();
            try
            {
              
                var msg = new MessageModel();
                if (model.UserId == 0)
                {
                    bool existuser = userBaseService.JudgeUserExist(model.UserCode);
                    if (existuser)
                    {
                        result.Msg = "用户名已存在";
                        return Json(result);
                    }
                    model.Password = EncodeStrToMd5.String32ToMD5("123456");
                    msg = userBaseService.InsertUserBase(model);
                }
                else
                {
                   msg = userBaseService.UpdateUserBase(model);
                }
                if (msg.Status == MessageStatus.Success)
                {
                    result.IsSuccess = true;
                }
                else
                {
                    result.Msg = "保存数据失败";
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }
        /// <summary>
        /// 修改用户数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult UpdateUser(UserDetailModel model)
        {
            bool isSuccess = false;
            try
            {
                userDetailService.InsertSingleUserBase(model);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return Json(isSuccess);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetUserList(int page, int rows)
        {
            int comId = 0;
            int deptId = 0;
            var id = RequestHelper.RequestPost("id", "0");
            var type = RequestHelper.RequestPost("type", "0");
            if (type =="0")
            {
                comId = Convert.ToInt32(id);
            }
            if (type == "1")
            {
                deptId = Convert.ToInt32(id);
            } 

            var uName = RequestHelper.RequestPost("uName", string.Empty);

            var userlist = userBaseService.GetUserBasePaging(comId,page, rows, deptId, uName);
            return Json(userlist);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(int id)
        {
            var result = new ResultJson();
            try
            {
                var msg = userBaseService.DeleteSingleUserBase(id);
                if (msg.Status == MessageStatus.Success)
                {
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
             Logger.Error(ex.ToString());
            }
            return Json(result);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DeleteUser(string ids)
        {
            var result = new ResultJson();
            try
            {
                var msg = userBaseService.DeleteUser(ids);
                if (msg.Status == MessageStatus.Success)
                {
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return Json(result);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult ResetUserPwd(string ids, string pwd)
        {
            var result = new ResultJson();
            try
            {
                string newPwd = EncodeStrToMd5.String32ToMD5(pwd);
                var msg = userBaseService.ResetUserPwd(ids, newPwd);
                if (msg.Status == MessageStatus.Success)
                {
                    result.IsSuccess = true;

                }
            }
            catch (Exception)
            {
 
            }

            return Json(result);
        }

        /// <summary>
        /// 获取公司部门机构数
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCompanyTree()
        {
            var result = new ResultJson();
            try
            {
                var data = companyService.GetCompantTree();
                result.IsSuccess = true;
                result.Data = data;
            }
            catch (Exception)
            {
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListInfo()
        {
            //UserManagement.CurrentCompany = new Model.CompanyModel() { CompanyID = 1 };
            var companylist = companyService.GetPagerCompany(string.Empty, 1, 100).Select(_ => new { CompanyName = _.CompanyName, CompanyID = _.CompanyID });
            var deptlist = deptService.GetPagerDept(string.Empty, 1, 100).Select(_ => new { DeptName = _.DeptName, DeptID = _.DeptID });
            return Json(new  { companys= companylist, depts= deptlist },  JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(UserDetailModel model, List<int> chkMenu)
        {
            try
            {
                // TODO: Add insert logic here

                //var result = userBaseService.InsertSingleUserBase(model.UserBase);
                //model.UserID = model.UserBase.UserID;
                //var result2 = userDetailService.InsertSingleUserBase(model);
                //var result3 = userRoleIndexService.InsertSingleEntity();
               // var result = userBaseService.InsertSingleEntity(model.UserBase, model, chkMenu);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}