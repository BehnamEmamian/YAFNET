/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  using HistoryEventArgs = nStuff.UpdateControls.HistoryEventArgs;

  #endregion

  /// <summary>
  /// Summary description for topics.
  /// </summary>
  public partial class search : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    /// The _search handled.
    /// </summary>
    private bool _searchHandled = false;

    /// <summary>
    /// The _search what cleaned.
    /// </summary>
    private string _searchWhatCleaned;

    /// <summary>
    /// The _search who cleaned.
    /// </summary>
    private string _searchWhoCleaned;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="search"/> class. 
    /// The search page constructor.
    /// </summary>
    public search()
      : base("SEARCH")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets HighlightSearchWords.
    /// </summary>
    protected List<string> HighlightSearchWords
    {
      get
      {
        if (this.ViewState["HighlightWords"] == null)
        {
          this.ViewState["HighlightWords"] = new List<string>();
        }

        return this.ViewState["HighlightWords"] as List<string>;
      }

      set
      {
        this.ViewState["HighlightWords"] = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether SearchHandled.
    /// </summary>
    protected bool SearchHandled
    {
      get
      {
        return this._searchHandled;
      }

      set
      {
        this._searchHandled = value;
      }
    }

    /// <summary>
    /// Gets or sets SearchWhatCleaned.
    /// </summary>
    protected string SearchWhatCleaned
    {
      get
      {
        if (this._searchWhatCleaned == null)
        {
          this._searchWhatCleaned =
            StringHelper.RemoveMultipleSingleQuotes(
              StringHelper.RemoveMultipleWhitespace(this.txtSearchStringWhat.Text.Trim()));
        }

        return this._searchWhatCleaned;
      }

      set
      {
        this._searchWhatCleaned = value;
      }
    }

    /// <summary>
    /// Gets or sets SearchWhoCleaned.
    /// </summary>
    protected string SearchWhoCleaned
    {
      get
      {
        if (this._searchWhoCleaned == null)
        {
          this._searchWhoCleaned =
            StringHelper.RemoveMultipleSingleQuotes(
              StringHelper.RemoveMultipleWhitespace(this.txtSearchStringFromWho.Text.Trim()));
        }

        return this._searchWhoCleaned;
      }

      set
      {
        this._searchWhoCleaned = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The btn search_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchUpdatePanel.Visible = true;
      this.SearchBindData(true);
    }

    /// <summary>
    /// The is search text too large.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The is search text too large.
    /// </returns>
    protected bool IsSearchTextTooLarge(string text)
    {
      if (text.Length > this.PageContext.BoardSettings.SearchStringMaxLength)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The is search text too small.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The is search text too small.
    /// </returns>
    protected bool IsSearchTextTooSmall(string text)
    {
      if (text.Length < this.PageContext.BoardSettings.SearchStringMinLength)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The is valid search request.
    /// </summary>
    /// <returns>
    /// The is valid search request.
    /// </returns>
    protected bool IsValidSearchRequest()
    {
      bool whatValid = this.IsValidSearchText(this.SearchWhatCleaned);
      bool whoValid = this.IsValidSearchText(this.SearchWhoCleaned);

      // they are both valid...
      if (whoValid && whatValid)
      {
        return true;
      }

      if (!whoValid && whatValid)
      {
        // makes sure no value is used...
        this.SearchWhoCleaned = String.Empty;

        // valid search...
        return true;
      }

      // !whatValid is always true... could be removed but left for clarity.
      if (whoValid && !whatValid)
      {
        // make sure no value is used...
        this.SearchWhatCleaned = String.Empty;

        // valid search...
        return true;
      }

      bool searchTooSmall = false;
      bool searchTooLarge = false;

      if (String.IsNullOrEmpty(this.SearchWhoCleaned) && this.IsSearchTextTooSmall(this.SearchWhatCleaned))
      {
        searchTooSmall = true;
      }
      else if (String.IsNullOrEmpty(this.SearchWhatCleaned) && this.IsSearchTextTooSmall(this.SearchWhoCleaned))
      {
        searchTooSmall = true;
      }
      else if (String.IsNullOrEmpty(this.SearchWhoCleaned) && this.IsSearchTextTooLarge(this.SearchWhatCleaned))
      {
        searchTooLarge = true;
      }
      else if (String.IsNullOrEmpty(this.SearchWhatCleaned) && this.IsSearchTextTooLarge(this.SearchWhoCleaned))
      {
        searchTooLarge = true;
      }

      // search may not be valid for some reason...
      if (searchTooSmall)
      {
        this.PageContext.AddLoadMessage(
          this.GetTextFormatted("SEARCH_CRITERIA_ERROR_MIN", this.PageContext.BoardSettings.SearchStringMinLength));
      }
      else if (searchTooLarge)
      {
        this.PageContext.AddLoadMessage(
          this.GetTextFormatted("SEARCH_CRITERIA_ERROR_MAX", this.PageContext.BoardSettings.SearchStringMaxLength));
      }

      return false;
    }

    /// <summary>
    /// The is valid search text.
    /// </summary>
    /// <param name="searchText">
    /// The search text.
    /// </param>
    /// <returns>
    /// The is valid search text.
    /// </returns>
    protected bool IsValidSearchText(string searchText)
    {
      return !String.IsNullOrEmpty(searchText) && !this.IsSearchTextTooSmall(searchText) &&
             !this.IsSearchTextTooLarge(searchText) &&
             Regex.IsMatch(searchText, this.PageContext.BoardSettings.SearchStringPattern);
    }

    /// <summary>
    /// The loading image_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LoadingImage_Load(object sender, EventArgs e)
    {
      this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loading-white.gif");
    }

    /// <summary>
    /// The loading modal text_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LoadingModalText_Load(object sender, EventArgs e)
    {
      this.LoadingModalText.Text = this.GetText("LOADING_SEARCH");
    }

    /// <summary>
    /// The on update history navigate.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void OnUpdateHistoryNavigate(object sender, HistoryEventArgs e)
    {
      int pageNumber, pageSize;

      string[] pagerData = e.EntryName.Split('|');

      if (pagerData.Length >= 2 && int.TryParse(pagerData[0], out pageNumber) &&
          int.TryParse(pagerData[1], out pageSize) && Mession.SearchData != null)
      {
        // use existing page...
        this.Pager.CurrentPageIndex = pageNumber;

        // and existing page size...
        this.Pager.PageSize = pageSize;

        // count...
        this.Pager.Count = Mession.SearchData.DefaultView.Count;

        // bind existing search
        this.SearchBindData(false);

        // use existing search data...
        this.SearchUpdatePanel.Update();
      }
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        this.btnSearch.Text = this.GetText("btnsearch");

        // Load result dropdown
        this.listResInPage.Items.Add(new ListItem(this.GetText("result5"), "5"));
        this.listResInPage.Items.Add(new ListItem(this.GetText("result10"), "10"));
        this.listResInPage.Items.Add(new ListItem(this.GetText("result25"), "25"));
        this.listResInPage.Items.Add(new ListItem(this.GetText("result50"), "50"));

        // Load searchwhere dropdown
        // listSearchWhere.Items.Add( new ListItem( GetText( "posts" ), "0" ) );
        // listSearchWhere.Items.Add( new ListItem( GetText( "postedby" ), "1" ) );

        // Load listSearchFromWho dropdown
        this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_all"), "0"));
        this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_any"), "1"));
        this.listSearchFromWho.Items.Add(new ListItem(this.GetText("match_exact"), "2"));

        // Load listSearchWhat dropdown
        this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_all"), "0"));
        this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_any"), "1"));
        this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_exact"), "2"));

        this.listSearchFromWho.SelectedIndex = 0;
        this.listSearchWhat.SelectedIndex = 0;

        // Load forum's combo
        // listForum.Items.Add( new ListItem( GetText( "allforums" ), "-1" ) );
        // DataTable dt = YAF.Classes.Data.DB.forum_listread( PageContext.PageBoardID, PageContext.PageUserID, null, null );

        // int nOldCat = 0;
        // for ( int i = 0; i < dt.Rows.Count; i++ )
        // {
        // DataRow row = dt.Rows [i];
        // if ( ( int ) row ["CategoryID"] != nOldCat )
        // {
        // nOldCat = ( int ) row ["CategoryID"];
        // listForum.Items.Add( new ListItem( ( string ) row ["Category"], "-1" ) );
        // }
        // listForum.Items.Add( new ListItem( " - " + ( string ) row ["Forum"], row ["ForumID"].ToString() ) );
        // }
        this.LoadingModal.Title = this.GetText("LOADING");

        this.listForum.DataSource = DB.forum_listall_sorted(this.PageContext.PageBoardID, this.PageContext.PageUserID);
        this.listForum.DataValueField = "ForumID";
        this.listForum.DataTextField = "Title";
        this.listForum.DataBind();
        this.listForum.Items.Insert(0, new ListItem(this.GetText("allforums"), "0"));

        bool doSearch = false;

        string searchString = this.Request.QueryString["search"];
        if (!String.IsNullOrEmpty(searchString) && searchString.Length < 50)
        {
          this.txtSearchStringWhat.Text = searchString;
          doSearch = true;
        }

        string postedBy = this.Request.QueryString["postedby"];
        if (!String.IsNullOrEmpty(postedBy) && postedBy.Length < 50)
        {
          this.txtSearchStringFromWho.Text = postedBy;
          doSearch = true;
        }

        // set the search box size via the max settings in the boardsettings.
        if (this.PageContext.BoardSettings.SearchStringMaxLength > 0)
        {
          this.txtSearchStringWhat.MaxLength = this.PageContext.BoardSettings.SearchStringMaxLength;
          this.txtSearchStringFromWho.MaxLength = this.PageContext.BoardSettings.SearchStringMaxLength;
        }

        if (doSearch)
        {
          this.SearchBindData(true);
        }
      }
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange(object sender, EventArgs e)
    {
      this.SmartScroller1.RegisterStartupReset();
      this.SearchBindData(false);
    }

    /// <summary>
    /// The search res_ item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SearchRes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      var cell = (HtmlTableCell)e.Item.FindControl("CounterCol");
      if (cell != null)
      {
        string messageID = cell.InnerText;
        int rowCount = e.Item.ItemIndex + 1 + (this.Pager.CurrentPageIndex * this.Pager.PageSize);
        cell.InnerHtml = string.Format(
          "<a href=\"{1}\">{0}</a>", rowCount, YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", messageID));
      }
    }

    /// <summary>
    /// The get server form.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    /// <returns>
    /// </returns>
    private HtmlForm GetServerForm(ControlCollection parent)
    {
      HtmlForm tmpHtmlForm = null;

      foreach (Control child in parent)
      {
        Type t = child.GetType();
        if (t == typeof(HtmlForm))
        {
          return (HtmlForm)child;
        }

        if (child.HasControls())
        {
          tmpHtmlForm = this.GetServerForm(child.Controls);
          if (tmpHtmlForm != null && tmpHtmlForm.ClientID != null)
          {
            return tmpHtmlForm;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// The search bind data.
    /// </summary>
    /// <param name="newSearch">
    /// The new search.
    /// </param>
    private void SearchBindData(bool newSearch)
    {
      try
      {
        if (newSearch && !this.IsValidSearchRequest())
        {
          return;
        }
        else if (newSearch || Mession.SearchData == null)
        {
          var sw = (SearchWhatFlags)Enum.Parse(typeof(SearchWhatFlags), this.listSearchWhat.SelectedValue);
          var sfw = (SearchWhatFlags)Enum.Parse(typeof(SearchWhatFlags), this.listSearchFromWho.SelectedValue);
          int forumID = int.Parse(this.listForum.SelectedValue);

          DataTable searchDataTable = DB.GetSearchResult(
            this.SearchWhatCleaned, 
            this.SearchWhoCleaned, 
            sfw, 
            sw, 
            forumID, 
            this.PageContext.PageUserID, 
            this.PageContext.PageBoardID, 
            this.PageContext.BoardSettings.ReturnSearchMax, 
            this.PageContext.BoardSettings.UseFullTextSearch);
          
          if (newSearch)
          {
            // setup highlighting
            this.SetupHighlightWords(sw);
          }

          this.Pager.CurrentPageIndex = 0;
          this.Pager.PageSize = int.Parse(this.listResInPage.SelectedValue);
          this.Pager.Count = searchDataTable.DefaultView.Count;
          Mession.SearchData = searchDataTable;

          bool areResults = (searchDataTable.DefaultView.Count > 0) ? true : false;

          this.SearchRes.Visible = areResults;
          this.NoResults.Visible = !areResults;
        }

        var pds = new PagedDataSource
          {
            AllowPaging = true,
            DataSource = Mession.SearchData.DefaultView,
            PageSize = this.Pager.PageSize,
            CurrentPageIndex = this.Pager.CurrentPageIndex
          };

        this.UpdateHistory.AddEntry(this.Pager.CurrentPageIndex.ToString() + "|" + this.Pager.PageSize);

        this.SearchRes.DataSource = pds;
        this.SearchRes.DataBind();
      }
      catch (Exception x)
      {
        DB.eventlog_create(this.PageContext.PageUserID, this, x);

        if (this.PageContext.IsAdmin)
        {
          this.PageContext.AddLoadMessage(string.Format("{0}", x));
        }
        else
        {
          this.PageContext.AddLoadMessage("An error occured while searching.");
        }
      }
    }

    /// <summary>
    /// Sets up highlighting of the search keywords.
    /// </summary>
    /// <param name="searchFlags">
    /// The search Flags.
    /// </param>
    private void SetupHighlightWords(SearchWhatFlags searchFlags)
    {
      if (this.HighlightSearchWords.Count == 0)
      {
        if (searchFlags == SearchWhatFlags.ExactMatch)
        {
          this.HighlightSearchWords.Add(this.SearchWhatCleaned);
        }
        else if (searchFlags == SearchWhatFlags.AnyWords || searchFlags == SearchWhatFlags.AllWords)
        {
          this.HighlightSearchWords.AddRange(
            this.SearchWhatCleaned.Split(' ').ToList().Where(x => !x.IsNullOrEmptyTrimmed()));
        }
      }
    }

    #endregion
  }
}