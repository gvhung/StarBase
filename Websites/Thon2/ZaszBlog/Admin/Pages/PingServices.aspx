<%@ Page AutoEventWireup="true" CodeFile="PingServices.aspx.cs" Inherits="AdminPingServices"
    Language="C#" MasterPageFile="~/ZaszBlog/Admin/ZaszBlogAdmin.master"
    Title="Ping services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
  <asp:TextBox runat="Server" ID="txtNewCategory" Width="300" />  
  <asp:Button runat="server" ID="btnAdd" ValidationGroup="new" />
  <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="Server" ValidationGroup="new" ControlToValidate="txtNewCategory" ErrorMessage="Please specify a ping service" /><br /><hr />


  <asp:GridView runat="server" ID="grid" CssClass="category"  
    UseAccessibleHeader="true" 
    GridLines="None"
    AutoGenerateColumns="false" 
    AlternatingRowStyle-CssClass="alt" 
    AutoGenerateDeleteButton="true" 
    AutoGenerateEditButton="true">
    <Columns>      
      <asp:TemplateField HeaderText="Name">
        <ItemTemplate>
          <%# Eval("value") %>
        </ItemTemplate>
        <EditItemTemplate>
          <asp:TextBox runat="server" ID="txtName" Text='<%# Eval("value") %>' />
        </EditItemTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
</asp:Content>
