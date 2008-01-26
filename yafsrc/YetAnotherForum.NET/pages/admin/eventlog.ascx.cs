/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for attachments.
	/// </summary>
	public partial class eventlog : YAF.Classes.Base.AdminPage
	{
		#region Event Handlers

		/// <summary>
		/// Page load event handler.
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// do it only once, not on postbacks
			if (!IsPostBack)
			{
				// create page links
				// board index first
				PageLinks.AddLink(PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum));
				// administration index second
				PageLinks.AddLink("Administration", YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.admin_admin));
				// we are now in event log
				PageLinks.AddLink("Event Log", "");

				// bind data to controls
				BindData();
			}
		}

		/// <summary>
		/// Handles load event for log entry delete link button.
		/// </summary>
		/// <remarks>Adds confirmation popup to click event of this button.</remarks>
		protected void Delete_Load(object sender, EventArgs e)
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this event log entry?')";
		}


		/// <summary>
		/// Handles load event for delete all button.
		/// </summary>
		/// <remarks>Adds confirmation popup to click event of this button.</remarks>
		protected void DeleteAll_Load(object sender, EventArgs e)
		{
			((Button)sender).Attributes["onclick"] = "return confirm('Delete all event log entries?')";
		}


		/// <summary>
		/// Handles delete all button on click event.
		/// </summary>
		protected void DeleteAll_Click(object sender, EventArgs e)
		{
			// delete all event log entries of this board
			YAF.Classes.Data.DB.eventlog_delete(PageContext.PageBoardID);
			// re-bind controls
			BindData();
		}

		/// <summary>
		/// Handles single record commands in a repeater.
		/// </summary>
		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			// what command are we serving?
			switch (e.CommandName)
			{
				// delete log entry
				case "delete":
					// delete just this particular log entry
					YAF.Classes.Data.DB.eventlog_delete(e.CommandArgument);
					// re-bind controls
					BindData();
					break;

				// show/hide log entry details
				case "show":
					// get details control
					Control ctl = e.Item.FindControl("details");
					// find link button control
					LinkButton showbutton = e.Item.FindControl("showbutton") as LinkButton;

					// change visibility state of detail and label of linkbutton too
					if (ctl.Visible = !ctl.Visible)
						showbutton.Text = "Hide";
					else
						showbutton.Text = "Show";
					break;
			}
		}
		
		#endregion


		#region Private Methods

		/// <summary>
		/// Populates data source and binds data to controls.
		/// </summary>
		private void BindData()
		{
			// list event for this board
			List.DataSource = YAF.Classes.Data.DB.eventlog_list(PageContext.PageBoardID);

			// bind data to controls
			DataBind();
		}


		/// <summary>
		/// Gets HTML IMG code representing given log event icon.
		/// </summary>
		/// <param name="dataRow">Data row containing event log entry data.</param>
		/// <returns>return HTML code of event log entry image</returns>
		protected string EventImageCode(object dataRow)
		{
			// cast object to the DataRowView
			DataRowView row = (DataRowView)dataRow;
			// set defaults
			string imageName = "eventError.gif";
			string imageType = "Error";

			// find out of what type event log entry is
			switch ((int)row["Type"])
			{
				// it's warning
				case 1:
					imageName = "eventWarning.gif";
					imageType = "Warning";
					break;

				// it's information
				case 2:
					imageName = "eventInfo.gif";
					imageType = "Information";
					break;
			}

			// return HTML code of event log entry image
			return String.Format(
						"<img src=\"images/{0}\" width=\"16\" height=\"16\" alt=\"{1}\" title=\"{0}\" />",
						imageName,
						imageType
						);
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
