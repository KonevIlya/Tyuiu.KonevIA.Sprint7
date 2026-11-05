namespace Tyuiu.KonevIA.Sprint7.Project.V10;
internal sealed class FormHelp_KIA : Form
{
    public FormHelp_KIA(string text,string title)
    {
        Name="formHelp_KIA";Text=title;StartPosition=FormStartPosition.CenterParent;
        Font=new Font("Segoe UI",10F);ClientSize=new Size(850,540);MinimumSize=new Size(730,510);
        BackColor=Color.FromArgb(243,246,250);ShowInTaskbar=false;
        var layout_KIA=new TableLayoutPanel{Name="tableLayoutHelp_KIA",Dock=DockStyle.Fill,Padding=new Padding(24),ColumnCount=2,RowCount=3};
        layout_KIA.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,220));layout_KIA.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,65));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,58));
        var heading_KIA=new Label{Name="labelHelpHeading_KIA",Text=title.StartsWith("О программе")?"О приложении":"Руководство пользователя",Font=new Font("Segoe UI",19F,FontStyle.Bold),ForeColor=Color.FromArgb(30,59,88),Dock=DockStyle.Fill,TextAlign=ContentAlignment.TopLeft,Margin=new Padding(0)};
        layout_KIA.Controls.Add(heading_KIA,0,0);layout_KIA.SetColumnSpan(heading_KIA,2);
        var profile_KIA=new TableLayoutPanel{Name="tableLayoutAuthor_KIA",Dock=DockStyle.Fill,ColumnCount=1,RowCount=3,BackColor=Color.White,Padding=new Padding(16),Margin=new Padding(0,0,20,0)};
        profile_KIA.RowStyles.Add(new RowStyle(SizeType.Percent,100));profile_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,58));profile_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,28));
        var photo_KIA=new PictureBox{Name="pictureBoxAuthor_KIA",Dock=DockStyle.Fill,SizeMode=PictureBoxSizeMode.Zoom,Margin=new Padding(0,0,0,12),AccessibleName="Фотография автора приложения"};
        using(var stream_KIA=typeof(FormHelp_KIA).Assembly.GetManifestResourceStream("AuthorPhoto_KIA"))
        using(var image_KIA=Image.FromStream(stream_KIA))photo_KIA.Image=new Bitmap(image_KIA);
        FormClosed+=(s,e)=>photo_KIA.Image?.Dispose();
        profile_KIA.Controls.Add(photo_KIA,0,0);
        profile_KIA.Controls.Add(new Label{Name="labelAuthor_KIA",Text="Конев Илья\r\nАндреевич",Font=new Font("Segoe UI",11F,FontStyle.Bold),Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Color.FromArgb(35,75,110),Margin=Padding.Empty},0,1);
        profile_KIA.Controls.Add(new Label{Name="labelAuthorGroup_KIA",Text="Группа НТм-25-1",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Color.DimGray,Margin=Padding.Empty},0,2);
        var contentPanel_KIA=new Panel{Name="panelHelpContent_KIA",Dock=DockStyle.Fill,Padding=new Padding(22),BackColor=Color.White,Margin=Padding.Empty};
        string normalized_KIA=text.Replace("\r\n","\n").Replace("\r","\n");
        var content_KIA=new RichTextBox{Name="richTextBoxHelpContent_KIA",Text=normalized_KIA.Replace("\n","\n\n"),ReadOnly=true,DetectUrls=false,ScrollBars=RichTextBoxScrollBars.Vertical,Dock=DockStyle.Fill,BorderStyle=BorderStyle.None,BackColor=Color.White,ForeColor=Color.FromArgb(52,65,80),Font=new Font("Segoe UI",10.5F),TabStop=false};
        contentPanel_KIA.Controls.Add(content_KIA);
        var close_KIA=new Button{Name="buttonCloseHelp_KIA",Text="Закрыть",DialogResult=DialogResult.OK,Anchor=AnchorStyles.Right|AnchorStyles.Bottom,Size=new Size(116,36),FlatStyle=FlatStyle.Flat,BackColor=Color.FromArgb(35,105,170),ForeColor=Color.White,Cursor=Cursors.Hand,Margin=Padding.Empty};
        close_KIA.FlatAppearance.BorderColor=Color.FromArgb(35,105,170);
        layout_KIA.Controls.Add(profile_KIA,0,1);layout_KIA.Controls.Add(contentPanel_KIA,1,1);layout_KIA.Controls.Add(close_KIA,1,2);
        Controls.Add(layout_KIA);AcceptButton=close_KIA;CancelButton=close_KIA;
    }
}
