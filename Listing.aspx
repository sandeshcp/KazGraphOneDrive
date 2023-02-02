<%@ Page Title="Listing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Listing.aspx.cs" Inherits="KazGraph.Listing" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%--<div class="jumbotron">
        <h1>OneDrive Listing POC</h1>
        <p class="lead">Display and Refresh File Listing</p>
        
    </div> --%>
    <style>
        .Grid th {
            background-color: #DF7401;
            color: White;
            font-size: 10pt;
            line-height: 200%;
        }

        .Grid td {
            background-color: #F3E2A9;
            color: black;
            font-size: 10pt;
            line-height: 200%;
            text-align: center;
        }

        .ChildGrid th {
            background-color: Maroon;
            color: White;
            font-size: 10pt;
        }

        .ChildGrid td {
            background-color: Orange;
            color: black;
            font-size: 10pt;
            text-align: center;
        }

        .GridViewClass td, th {
            cellpadding-left: 5px;
        }
    </style>



    <h3>OneDrive Listing POC</h3>

    <p>Show Treeview control below with latest file listing</p>
    <p>
        <asp:Button ID="btnGetOneDriveFileListing" runat="server" Text="Load FileList from OneDrive and display in below table " OnClick="btnGetOneDriveFileListing_Click" />
        <asp:Label ID="lblGetOneDriveFileListing" runat="server" Text="."></asp:Label>
    </p>
    <div>
        <asp:GridView ID="grvODrive" AllowPaging="true" PageSize="5" HeaderStyle-CssClass="bg-primary text-white"
            OnPageIndexChanging="grvODrive_PageIndexChanging" AutoGenerateColumns="false"
            runat="server"
            OnRowDataBound="grvODrive_RowDataBound">
            <RowStyle BackColor="#EFF3FB" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <%--<asp:CommandField ShowSelectButton="true" ButtonType="Button" />--%>
                <asp:TemplateField HeaderText="Row Number" ItemStyle-Width="100">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField ItemStyle-Width="200" DataField="id" HeaderText="ID" />
                <asp:TemplateField HeaderText="Web Url To Access">
                    <ItemTemplate>
                        <img src='<%# Convert.ToString(Eval("folder")) == "KazGraph.Models.Folder" ? "img/folder.svg" : "" %>' style="width: 20px;">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("webUrl") %>' Target="_blank"><%# Eval("name") %>(<%# Eval("size") %>)</asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField ItemStyle-Width="200" DataField="eTag" HeaderText="eTag" />
                <asp:BoundField ItemStyle-Width="200" DataField="cTag" HeaderText="cTag" />
                <asp:BoundField ItemStyle-Width="200" DataField="createdDateTime" HeaderText="Created Date" />
                <asp:BoundField ItemStyle-Width="200" DataField="lastModifiedDateTime" HeaderText="Modified Date" />
                <asp:BoundField ItemStyle-Width="200" DataField="parentReference.path" HeaderText="parentReference path" />
                <asp:BoundField ItemStyle-Width="200" DataField="file.mimeType" HeaderText="File Structure" />
                <%--<asp:BoundField ItemStyle-Width="200" DataField="ChildNode" HeaderText="Child Node" />--%>

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:GridView ID="gv2" runat="server" AutoGenerateColumns="false" CssClass="ChildGrid"
                            DataKeyNames="parentReference.path">
                            <Columns>
                                <asp:TemplateField HeaderText="Web Url To Access">
                                    <ItemTemplate>
                                        <img src='<%# Convert.ToString(Eval("folder")) == "KazGraph.Models.Folder" ? "img/folder.svg" : "" %>' style="width: 20px;">
                                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("webUrl") %>' Target="_blank"><%# Eval("name") %>(<%# Eval("size") %>)</asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
    </div>
    <br />
    <hr />

    <p>
        <asp:Button ID="btnPopulateTable" runat="server" Text="Show Latest Listing in Table below from db Kazoo table FileList " OnClick="btnPopulateTable_Click" />
        <asp:Label ID="lblPopulateTable" runat="server" Text="."></asp:Label>
    </p>
    <p>

        <asp:TreeView ID="TreeView1" runat="server">
        </asp:TreeView>

    </p>
    <p></p>




</asp:Content>

