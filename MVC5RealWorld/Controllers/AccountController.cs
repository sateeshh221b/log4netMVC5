using System.Web.Mvc;
using System.Web.Security;
using MVC5RealWorld.Models.ViewModel;
using MVC5RealWorld.Models.EntityManager;
using System;
using log4net;

namespace MVC5RealWorld.Controllers
{
    public class AccountController : Controller
    {
        readonly ILog logger;

        public AccountController()
        {
            logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserSignUpView USV)
        {
            if (ModelState.IsValid)
            {
                //Log.Log.Info($"{USV.LoginName} wants to signup, wants to have role of {USV.RoleName}");
                logger.Info($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - {USV.LoginName} wants to signup, wants to have role of {USV.RoleName}");

                UserManager UM = new UserManager();
                if (!UM.IsLoginNameExist(USV.LoginName))
                {
                    UM.AddUserAccount(USV);
                    FormsAuthentication.SetAuthCookie(USV.FirstName, false);
                    logger.Info($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - SignUp success for {USV.LoginName}");
                    return RedirectToAction("Welcome", "Home");
                }
                else
                {
                    logger.Error($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - SignUp failed for {USV.LoginName}, login name already taken");
                    ModelState.AddModelError("", "Login Name already taken.");
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(UserLoginView ULV, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Log.Log.Info($"{ULV.LoginName} initiated a login");

                UserManager UM = new UserManager();
                string password = UM.GetUserPassword(ULV.LoginName);

                if (string.IsNullOrEmpty(password))
                {
                    //Log.Log.Error($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - {ULV.LoginName} login or password provided is incorrect ");
                    ModelState.AddModelError("", "The user login or password provided is incorrect.");
                }
                else
                {
                    if (ULV.Password.Equals(password))
                    {
                        //Log.Log.Info($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - {ULV.LoginName} login successfull");
                        //FormsAuthentication.SetAuthCookie(ULV.LoginName, false);
                        //return RedirectToAction("Welcome", "Home");
                        FormsAuthentication.RedirectFromLoginPage(ULV.LoginName, false);
                    }
                    else
                    {
                        //Log.Log.Error($"{DateTime.Now.ToString("MMddyyyyhhmmss")} - {ULV.LoginName} - The password provided is incorrect");
                        ModelState.AddModelError("", "The password provided is incorrect.");
                    }
                }
            }

            //Log.Log.Error($"{ULV.LoginName} login failed at {DateTime.Now.ToString("MMddyyyyhhmmss")} got redirected to login page");
            return View(ULV);
        }
    }
}