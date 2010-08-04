<%@ Control Language="c#" AutoEventWireup="True" CodeFile="forumList.ascx.cs" Inherits="YAF.Controls.ForumList"
	EnableViewState="false" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="../../../controls/ForumSubForumList.ascx" %>
<asp:Repeater ID="ForumList1" runat="server" OnItemCreated="ForumList1_ItemCreated">
	<ItemTemplate>
		<tr class="forumRow post">
			<td class="forumIconCol">
				<YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div>
				<div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</div>
				<YAF:ForumSubForumList ID="SubForumList" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
					Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="forumRow_Alt post_alt">
			<td>
				<YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div>
				<div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</div>
				<YAF:ForumSubForumList ID="ForumSubForumListAlt" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
					Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
