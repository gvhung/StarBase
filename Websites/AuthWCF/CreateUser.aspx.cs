﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class CreateUser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void On_CreatedUser(object sender, EventArgs e)
    {
        string userName = CreateUserWizard1.UserName;
        if (CheckBox1.Checked)
        {
            HttpContext.Current.Response.Write(userName);
            Roles.AddUserToRole(userName, "Administrators");
        }
        else
            Roles.AddUserToRole(userName, "Friends");

        CheckBox1.Visible = false;

        HttpContext.Current.Response.Redirect("~/default.aspx");
    }
}