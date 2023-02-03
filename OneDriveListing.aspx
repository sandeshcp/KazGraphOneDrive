<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OneDriveListing.aspx.cs" Inherits="KazGraph.OneDriveListing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        body {
            font-family: Arial;
            font-size: 10pt;
        }

        select {
            -webkit-appearance: none;
            -moz-appearance: none;
            -ms-appearance: none;
            appearance: none;
            outline: 0;
            background: #3ac0f2;
            background-image: none;
            border: 1px solid black;
        }

        .select {
            position: relative;
            display: block;
            width: 20em;
            height: 3em;
            line-height: 3;
            background: #2C3E50;
            overflow: hidden;
            border-radius: .25em;
        }

        select {
            height: 100%;
            margin: 0;
            padding: 0 0 0 0.5em;
            color: #fff;
            cursor: pointer;
            padding: 5px;
        }

            select::-ms-expand {
                display: none;
            }

        .select::after {
            content: '\25BC';
            position: absolute;
            top: 0;
            right: 0;
            bottom: 0;
            padding: 0 1em;
            background: #34495E;
            pointer-events: none;
        }

        .select:hover::after {
            color: #F39C12;
        }

        <!-- For different browsers --> .select::after {
            -webkit-transition: .25s all ease;
            -o-transition: .25s all ease;
            transition: .25s all ease;
        }

        .Grid td {
            background-color: #e5ebed;
            color: black;
            font-size: 10pt;
            line-height: 200%
        }

        .Grid th {
            background-color: #3AC0F2;
            color: White;
            font-size: 10pt;
            line-height: 200%
        }

        .ChildGrid td {
            background-color: #eee !important;
            color: black;
            font-size: 10pt;
            line-height: 200%
        }

        .ChildGrid th {
            background-color: #6C6C6C !important;
            color: White;
            font-size: 10pt;
            line-height: 200%
        }

        .list-95 {
            white-space: nowrap;
            padding: 0px;
            margin: 0px 0px 0px -8px;
            display: flex;
            align-items: stretch;
        }

        .listItem-96 {
            list-style-type: none;
            margin: 0px;
            padding: 0px;
            display: flex;
            position: relative;
            align-items: center;
            flex: 0 0 auto;
            overflow: hidden;
        }

        .listItem-96 {
            list-style-type: none;
            margin: 0px;
            padding: 0px;
            display: flex;
            position: relative;
            align-items: center;
            flex: 0 0 auto;
            overflow: hidden;
        }

        .list-95 {
            white-space: nowrap;
            padding: 0px;
            margin: 0px 0px 0px -8px;
            display: flex;
            align-items: stretch;
        }

        .chevron-104 {
            display: inline-block;
            -webkit-font-smoothing: antialiased;
            font-style: normal;
            font-weight: normal;
            speak: none;
            font-family: FabricMDL2Icons;
            color: rgb(96, 94, 92);
            font-size: 12px;
        }

        .root_e4f70198 {
            position: relative;
            width: 100%;
        }

        .root-94 {
            font-family: "Segoe UI", "Segoe UI Web (West European)", "Segoe UI", -apple-system, BlinkMacSystemFont, Roboto, "Helvetica Neue", sans-serif;
            -webkit-font-smoothing: antialiased;
            font-size: 14px;
            font-weight: 400;
            margin: 0px;
        }
    </style>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "img/minus.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "img/plus.png");
            $(this).closest("tr").next().remove();
        });
    </script>

    <div>
        <div>
            <br />
            <br />
            Tenant ; 
            <asp:DropDownList ID="dllTenent" runat="server" AutoPostBack="True" DataTextField="displayName" DataValueField="id" OnSelectedIndexChanged="dllTenent_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
            Tenant User ; 
            <asp:DropDownList ID="ddlUser" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlUser_SelectedIndexChanged" DataTextField="displayName" DataValueField="id"></asp:DropDownList>
            <br />
            <br />
            <asp:DropDownList ID="ddlAction" runat="server" AutoPostBack="True" DataTextField="displayName" DataValueField="id" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                <%--<asp:ListItem Value="-1">Please Select</asp:ListItem>
                <asp:ListItem Value="1">From DB </asp:ListItem>
                <asp:ListItem Value="2">To DB</asp:ListItem>--%>
            </asp:DropDownList>
            <br />
            <br />
            <div class="ms-FocusZone css-61" data-focuszone-id="FocusZone48">
                <ol class="ms-Breadcrumb-list list-95">
                    <li class="ms-Breadcrumb-listItem listItem-96">
                        <div class="DropTargetRelay root_e4f70198">
                            <div class="DragSourceRelay root_e4f70198" draggable="true">
                                <div class="dropTarget_db32a283">
                                    <a class="ms-Link ms-Breadcrumb-itemLink itemLink-102" tabindex="0" href="OneDriveListing">
                                        <div class="">
                                            My files
                                        </div>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </li>
                    <li class="ms-Breadcrumb-listItem listItem-96">
                        <div class="DropTargetRelay root_e4f70198">
                            <div class="DragSourceRelay root_e4f70198" draggable="true">
                                <div class="dropTarget_db32a283">
                                    <a class="ms-Link ms-Breadcrumb-itemLink itemLink-102" tabindex="0">
                                        <div class="">
                                            <%= Convert.ToString(Request.QueryString["name"])!=null ? ">>"+Request.QueryString["name"].Replace("/drive/root:/","") : "" %>
                                            <%--<%=Request.QueryString["name"]%>--%>
                                        </div>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </li>
                </ol>
            </div>
        </div>
        <hr />

        <asp:GridView ID="gvOneDriveItem" runat="server" AutoGenerateColumns="false" CssClass="Grid"
            DataKeyNames="id" OnRowDataBound="OnRowDataBound" ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <img src='<%# Convert.ToString(Eval("folder")) == "KazGraph.Models.Folder" ? "img/plus.png" : "" %>' style="cursor: pointer;">

                        <%--<img alt="" style="cursor: pointer" src="img/plus.png" />--%>
                        <asp:Panel ID="pnlOneDrive" runat="server" Style="display: none">
                            <asp:GridView ID="gvOneDriveItemChild" runat="server" AutoGenerateColumns="false" CssClass="ChildGrid">
                                <Columns>
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
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Row Number" ItemStyle-Width="100">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField ItemStyle-Width="200" DataField="id" HeaderText="ID" />

                <asp:TemplateField HeaderText="One Drive URL/Download">
                    <ItemTemplate>
                        <img src='<%# Convert.ToString(Eval("folder")) == "KazGraph.Models.Folder" ? "img/folder.svg" : "" %>' style="width: 20px;">
                        <%--(<%# Eval("size") %>)--%>
                        <%--<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("webUrl") %>' Text='<%# Eval("name") %>' Target="_blank"></asp:HyperLink>--%>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Convert.ToString(Eval("folder")) == "KazGraph.Models.Folder" ? "OneDriveListing.aspx?name="+Eval("ChildNode") : Eval("webUrl") %>' Text='<%# Eval("Name") %>' Target="_self"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField ItemStyle-Width="200" DataField="eTag" HeaderText="eTag" />
                <asp:BoundField ItemStyle-Width="200" DataField="cTag" HeaderText="cTag" />
                <asp:BoundField ItemStyle-Width="200" DataField="createdDateTime" HeaderText="Created Date" />
                <asp:BoundField ItemStyle-Width="200" DataField="lastModifiedDateTime" HeaderText="Modified Date" />
                <asp:BoundField ItemStyle-Width="200" DataField="parentReference.path" HeaderText="parentReference path" />
                <asp:BoundField ItemStyle-Width="200" DataField="file.mimeType" HeaderText="File Structure" />
            </Columns>
            <EmptyDataTemplate>
                <div align="center">No records found.</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

</asp:Content>
