using System.ComponentModel;
using Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
namespace Tyuiu.KonevIA.Sprint7.Project.V10;
internal sealed class FormOrders_KIA : Form
{
    private readonly OrderService_KIA service_KIA=new();
    private readonly CsvRepository_KIA repository_KIA=new();
    private List<Order_KIA> orders_KIA=new();
    private List<Order_KIA> visible_KIA=new();
    private string filePath_KIA;
    private bool dirty_KIA;
    private readonly ToolTip toolTipControls_KIA=new();
    private readonly DataGridView dataGridViewOrders_KIA=new(){Name="dataGridViewOrders_KIA",Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,AutoGenerateColumns=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false,RowHeadersVisible=false,BackgroundColor=Color.White,BorderStyle=BorderStyle.None};
    private readonly TextBox textBoxSearch_KIA=new(){Name="textBoxSearch_KIA",Width=230,PlaceholderText="Номер, клиент, товар, телефон…"};
    private readonly ComboBox comboBoxStatus_KIA=new(){Name="comboBoxStatusFilter_KIA",Width=130,DropDownStyle=ComboBoxStyle.DropDownList};
    private readonly ComboBox comboBoxSort_KIA=new(){Name="comboBoxSortOrders_KIA",Width=125,DropDownStyle=ComboBoxStyle.DropDownList};
    private readonly CheckBox checkBoxDateFilter_KIA=new(){Name="checkBoxUseDateFilter_KIA",Text="Дата исполнения:",AutoSize=true};
    private readonly DateTimePicker dateTimePickerFrom_KIA=new(){Name="dateTimePickerFrom_KIA",Format=DateTimePickerFormat.Short,Width=110};
    private readonly DateTimePicker dateTimePickerTo_KIA=new(){Name="dateTimePickerTo_KIA",Format=DateTimePickerFormat.Short,Width=110};
    private readonly Label labelStatistics_KIA=new(){Name="labelStatistics_KIA",Dock=DockStyle.Fill,AutoSize=false,Padding=new Padding(16),Font=new Font("Segoe UI",11F)};
    private readonly OrderChart_KIA chartOrders_KIA=new();
    private readonly ToolStripStatusLabel statusLabelFile_KIA=new(){Name="toolStripStatusFile_KIA",Spring=true,TextAlign=ContentAlignment.MiddleLeft};
    private readonly ToolStripStatusLabel statusLabelCount_KIA=new(){Name="toolStripStatusCount_KIA"};
    private ToolStripButton buttonEdit_KIA,buttonDelete_KIA;
    public FormOrders_KIA()
    {
        Name="formOrders_KIA";Text="Заказы | Конев И. А. | Sprint7, вариант 10";ClientSize=new Size(1280,800);MinimumSize=new Size(1000,680);StartPosition=FormStartPosition.CenterScreen;Font=new Font("Segoe UI",10F);
        var menu_KIA=new MenuStrip{Name="menuStripMain_KIA"};
        var file_KIA=new ToolStripMenuItem("Файл"){Name="menuFile_KIA"};
        file_KIA.DropDownItems.Add(Menu("Новая база","New",(s,e)=>NewFile(),Keys.Control|Keys.N));
        file_KIA.DropDownItems.Add(Menu("Открыть CSV…","Open",(s,e)=>OpenFile(),Keys.Control|Keys.O));
        file_KIA.DropDownItems.Add(Menu("Сохранить","Save",(s,e)=>SaveFile(false),Keys.Control|Keys.S));
        file_KIA.DropDownItems.Add(Menu("Сохранить как…","SaveAs",(s,e)=>SaveFile(true)));
        file_KIA.DropDownItems.Add(Menu("Экспорт выборки…","Export",(s,e)=>Export()));
        file_KIA.DropDownItems.Add(Menu("Открыть пример","Example",(s,e)=>OpenExample()));
        file_KIA.DropDownItems.Add(Menu("Выход","Exit",(s,e)=>Close(),Keys.Alt|Keys.F4));
        var edit_KIA=new ToolStripMenuItem("Заказы"){Name="menuOrders_KIA"};
        edit_KIA.DropDownItems.Add(Menu("Добавить","Add",(s,e)=>EditOrder(false),Keys.Control|Keys.Insert));
        edit_KIA.DropDownItems.Add(Menu("Изменить","Edit",(s,e)=>EditOrder(true),Keys.Control|Keys.E));
        edit_KIA.DropDownItems.Add(Menu("Удалить","Delete",(s,e)=>DeleteOrder(),Keys.Control|Keys.Delete));
        var help_KIA=new ToolStripMenuItem("Справка"){Name="menuHelp_KIA"};help_KIA.DropDownItems.Add(Menu("Руководство пользователя","Guide",(s,e)=>Guide(),Keys.F1));help_KIA.DropDownItems.Add(Menu("О программе","About",(s,e)=>About()));
        menu_KIA.Items.AddRange(new ToolStripItem[]{file_KIA,edit_KIA,help_KIA});MainMenuStrip=menu_KIA;
        var toolbar_KIA=new ToolStrip{Renderer=new BorderedToolbarRenderer_KIA(),BackColor=Color.FromArgb(240,245,250),Name="toolStripActions_KIA",GripStyle=ToolStripGripStyle.Hidden,ImageScalingSize=new Size(24,24),Padding=new Padding(8)};
        toolbar_KIA.Items.Add(Tool("Открыть","Open",(s,e)=>OpenFile(),"Открыть базу заказов CSV (Ctrl+O)"));
        toolbar_KIA.Items.Add(Tool("Сохранить","Save",(s,e)=>SaveFile(false),"Сохранить всю базу (Ctrl+S)"));
        toolbar_KIA.Items.Add(new ToolStripSeparator{Name="separatorFileActions_KIA"});
        toolbar_KIA.Items.Add(Tool("Добавить","Add",(s,e)=>EditOrder(false),"Создать заказ"));
        buttonEdit_KIA=Tool("Изменить","Edit",(s,e)=>EditOrder(true),"Изменить выбранный заказ; также двойной щелчок по строке");toolbar_KIA.Items.Add(buttonEdit_KIA);
        buttonDelete_KIA=Tool("Удалить","Delete",(s,e)=>DeleteOrder(),"Удалить выбранный заказ после подтверждения");toolbar_KIA.Items.Add(buttonDelete_KIA);
        toolbar_KIA.Items.Add(new ToolStripSeparator{Name="separatorOrderActions_KIA"});toolbar_KIA.Items.Add(Tool("Экспорт","Export",(s,e)=>Export(),"Сохранить только строки текущей выборки"));toolbar_KIA.Items.Add(Tool("Помощь","Guide",(s,e)=>Guide(),"Краткое руководство (F1)"));
        var filters_KIA=new FlowLayoutPanel{Name="flowLayoutFilters_KIA",Dock=DockStyle.Fill,WrapContents=true,Padding=new Padding(8),AutoScroll=true};
        filters_KIA.Controls.Add(Label("Поиск:","Search"));filters_KIA.Controls.Add(textBoxSearch_KIA);
        comboBoxStatus_KIA.Items.Add("Все статусы");comboBoxStatus_KIA.Items.AddRange(OrderService_KIA.Statuses);comboBoxStatus_KIA.SelectedIndex=0;
        filters_KIA.Controls.Add(Label("Статус:","Status"));filters_KIA.Controls.Add(comboBoxStatus_KIA);
        comboBoxSort_KIA.Items.AddRange(new[]{"Дата ↑","Дата ↓","Сумма ↑","Сумма ↓","Клиент А–Я","Номер"});comboBoxSort_KIA.SelectedIndex=0;
        filters_KIA.Controls.Add(Label("Сортировка:","Sort"));filters_KIA.Controls.Add(comboBoxSort_KIA);
        filters_KIA.Controls.Add(checkBoxDateFilter_KIA);filters_KIA.Controls.Add(dateTimePickerFrom_KIA);filters_KIA.Controls.Add(Label("—","DateRange"));filters_KIA.Controls.Add(dateTimePickerTo_KIA);
        var reset_KIA=new Button{Name="buttonResetFilters_KIA",Text="Сбросить",AutoSize=true};reset_KIA.Click+=(s,e)=>{textBoxSearch_KIA.Clear();comboBoxStatus_KIA.SelectedIndex=0;checkBoxDateFilter_KIA.Checked=false;comboBoxSort_KIA.SelectedIndex=0;};reset_KIA.FlatStyle=FlatStyle.Flat;reset_KIA.FlatAppearance.BorderColor=Color.FromArgb(170,188,207);reset_KIA.BackColor=Color.White;reset_KIA.Cursor=Cursors.Hand;filters_KIA.Controls.Add(reset_KIA);
        AddColumn("Number","№ заказа",100);AddColumn("Customer","Клиент",220);AddColumn("Phone","Телефон",140);AddColumn("Product","Товар",190);AddColumn("DueDate","Исполнение",115,"dd.MM.yyyy");AddColumn("Price","Цена, ₽",115,"N2");AddColumn("Quantity","Кол-во",85);AddColumn("Total","Стоимость, ₽",135,"N2");AddColumn("Status","Статус",110);AddColumn("Account","Счёт",110);AddColumn("Address","Адрес",220);
        dataGridViewOrders_KIA.EnableHeadersVisualStyles=false;
        dataGridViewOrders_KIA.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(229,237,246);
        dataGridViewOrders_KIA.ColumnHeadersDefaultCellStyle.ForeColor=Color.FromArgb(35,55,75);
        dataGridViewOrders_KIA.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",10F,FontStyle.Bold);
        dataGridViewOrders_KIA.DefaultCellStyle.SelectionBackColor=Color.FromArgb(211,230,249);
        dataGridViewOrders_KIA.DefaultCellStyle.SelectionForeColor=Color.FromArgb(20,42,64);
        dataGridViewOrders_KIA.GridColor=Color.FromArgb(224,231,239);
        dataGridViewOrders_KIA.CellBorderStyle=DataGridViewCellBorderStyle.SingleHorizontal;
        dataGridViewOrders_KIA.BorderStyle=BorderStyle.FixedSingle;
        dataGridViewOrders_KIA.CellFormatting+=FormatCell;
        dataGridViewOrders_KIA.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(244,248,252);dataGridViewOrders_KIA.ColumnHeadersHeight=36;dataGridViewOrders_KIA.RowTemplate.Height=30;
        dataGridViewOrders_KIA.CellDoubleClick+=(s,e)=>{if(e.RowIndex>=0)EditOrder(true);};dataGridViewOrders_KIA.SelectionChanged+=(s,e)=>UpdateSelection();
        var lower_KIA=new SplitContainer{Name="splitContainerSummary_KIA",Dock=DockStyle.Fill,Size=new Size(1250,245),SplitterDistance=430};
        var statsBox_KIA=new GroupBox{Name="groupBoxStatistics_KIA",Text="Статистика текущей выборки",Dock=DockStyle.Fill};statsBox_KIA.Controls.Add(labelStatistics_KIA);lower_KIA.Panel1.Controls.Add(statsBox_KIA);lower_KIA.Panel2.Controls.Add(chartOrders_KIA);
        var mainSplit_KIA=new SplitContainer{Name="splitContainerOrdersSummary_KIA",Dock=DockStyle.Fill,Orientation=Orientation.Horizontal,Size=new Size(1250,580),SplitterDistance=330,Panel1MinSize=130,Panel2MinSize=170};mainSplit_KIA.Panel1.Controls.Add(dataGridViewOrders_KIA);mainSplit_KIA.Panel2.Controls.Add(lower_KIA);
        var layout_KIA=new TableLayoutPanel{Name="tableLayoutWorkspace_KIA",Dock=DockStyle.Fill,RowCount=2,ColumnCount=1};layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,88));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout_KIA.Controls.Add(filters_KIA,0,0);layout_KIA.Controls.Add(mainSplit_KIA,0,1);
        var status_KIA=new StatusStrip{Name="statusStripOrders_KIA"};status_KIA.Items.AddRange(new ToolStripItem[]{statusLabelFile_KIA,statusLabelCount_KIA});
        Controls.Add(layout_KIA);Controls.Add(status_KIA);Controls.Add(toolbar_KIA);Controls.Add(menu_KIA);toolbar_KIA.Dock=DockStyle.Top;menu_KIA.Dock=DockStyle.Top;
        textBoxSearch_KIA.TextChanged+=(s,e)=>RefreshOrders();comboBoxStatus_KIA.SelectedIndexChanged+=(s,e)=>RefreshOrders();comboBoxSort_KIA.SelectedIndexChanged+=(s,e)=>RefreshOrders();checkBoxDateFilter_KIA.CheckedChanged+=(s,e)=>RefreshOrders();dateTimePickerFrom_KIA.ValueChanged+=(s,e)=>RefreshOrders();dateTimePickerTo_KIA.ValueChanged+=(s,e)=>RefreshOrders();
        toolTipControls_KIA.SetToolTip(textBoxSearch_KIA,"Поиск без учёта регистра по номеру, ФИО, счёту, адресу, телефону и товару");toolTipControls_KIA.SetToolTip(comboBoxStatus_KIA,"Показывать заказы только выбранного статуса");toolTipControls_KIA.SetToolTip(comboBoxSort_KIA,"Порядок строк в таблице и экспорте");toolTipControls_KIA.SetToolTip(checkBoxDateFilter_KIA,"Включить фильтр по дате исполнения включительно");
        toolTipControls_KIA.SetToolTip(dateTimePickerFrom_KIA,"Начальная дата исполнения включительно");toolTipControls_KIA.SetToolTip(dateTimePickerTo_KIA,"Конечная дата исполнения включительно");toolTipControls_KIA.SetToolTip(dataGridViewOrders_KIA,"Выберите заказ. Двойной щелчок открывает карточку редактирования.");toolTipControls_KIA.SetToolTip(chartOrders_KIA,"Суммарная стоимость видимых заказов по статусам");
        FormClosing+=(s,e)=>{if(!ConfirmUnsaved())e.Cancel=true;};FormClosed+=(s,e)=>toolTipControls_KIA.Dispose();RefreshOrders();
    }
    private void FormatCell(object sender,DataGridViewCellFormattingEventArgs e)
    {
        if(e.RowIndex<0)return;
        string property_KIA=dataGridViewOrders_KIA.Columns[e.ColumnIndex].DataPropertyName;
        if(e.Value is string text_KIA&&string.IsNullOrWhiteSpace(text_KIA))
        {e.Value="—";e.FormattingApplied=true;e.CellStyle.BackColor=Color.FromArgb(255,249,230);e.CellStyle.ForeColor=Color.FromArgb(125,105,55);}
        if(property_KIA=="Status")
        {e.CellStyle.BackColor=e.Value?.ToString() switch {"Выполнен"=>Color.FromArgb(229,244,233),"В работе"=>Color.FromArgb(228,239,252),"Отменён"=>Color.FromArgb(246,232,232),_=>Color.FromArgb(255,247,219)};}
    }
    private static Label Label(string text,string name)=>new(){Name="label"+name+"_KIA",Text=text,AutoSize=true,Margin=new Padding(6,7,3,0)};
    private static ToolStripMenuItem Menu(string text,string name,EventHandler action,Keys shortcut=Keys.None){var item_KIA=new ToolStripMenuItem(text){Name="menu"+name+"_KIA",ShortcutKeys=shortcut};item_KIA.Click+=action;return item_KIA;}
    private static ToolStripButton Tool(string text,string name,EventHandler action,string tip){var button_KIA=new ToolStripButton(text){Name="button"+name+"_KIA",ToolTipText=tip,DisplayStyle=ToolStripItemDisplayStyle.Text,AutoSize=false,Width=100,Height=34,Margin=new Padding(3,2,3,2)};button_KIA.Click+=action;return button_KIA;}
    private void AddColumn(string property,string title,int width,string format=null){var column_KIA=new DataGridViewTextBoxColumn{Name="column"+property+"_KIA",DataPropertyName=property,HeaderText=title,Width=width,SortMode=DataGridViewColumnSortMode.NotSortable};if(format!=null)column_KIA.DefaultCellStyle.Format=format;dataGridViewOrders_KIA.Columns.Add(column_KIA);}
    private Order_KIA Selected()=>dataGridViewOrders_KIA.CurrentRow?.DataBoundItem as Order_KIA;
    private void UpdateSelection(){bool selected_KIA=Selected()!=null;buttonEdit_KIA.Enabled=selected_KIA;buttonDelete_KIA.Enabled=selected_KIA;}
    private void RefreshOrders()
    {
        dateTimePickerFrom_KIA.Enabled=dateTimePickerTo_KIA.Enabled=checkBoxDateFilter_KIA.Checked;
        bool invalidDates_KIA=checkBoxDateFilter_KIA.Checked&&dateTimePickerFrom_KIA.Value.Date>dateTimePickerTo_KIA.Value.Date;
        statusLabelCount_KIA.ForeColor=invalidDates_KIA?Color.Firebrick:SystemColors.ControlText;
        visible_KIA=service_KIA.Query(orders_KIA,textBoxSearch_KIA.Text,comboBoxStatus_KIA.SelectedIndex>0?comboBoxStatus_KIA.Text:null,checkBoxDateFilter_KIA.Checked?dateTimePickerFrom_KIA.Value:null,checkBoxDateFilter_KIA.Checked?dateTimePickerTo_KIA.Value:null,comboBoxSort_KIA.Text);
        dataGridViewOrders_KIA.DataSource=new BindingList<Order_KIA>(visible_KIA);
        var stat_KIA=service_KIA.Statistics(visible_KIA);labelStatistics_KIA.Text=$"Заказов: {stat_KIA.Count}      Товаров: {stat_KIA.Quantity}\n\nСумма: {stat_KIA.Sum:N2} ₽\nСредняя стоимость: {stat_KIA.Average:N2} ₽\nМинимум: {stat_KIA.Min:N2} ₽\nМаксимум: {stat_KIA.Max:N2} ₽";
        chartOrders_KIA.Orders=visible_KIA;chartOrders_KIA.Invalidate();statusLabelCount_KIA.Text=invalidDates_KIA?"Начальная дата позже конечной":$"Показано {visible_KIA.Count} из {orders_KIA.Count}";statusLabelFile_KIA.Text=(dirty_KIA?"● Изменено | ":"")+(filePath_KIA??"Новая база — не сохранена");UpdateSelection();
    }
    private void EditOrder(bool edit)
    {
        var selected_KIA=Selected();if(edit&&selected_KIA==null)return;
        using var dialog_KIA=new FormEditOrder_KIA(edit?selected_KIA:new Order_KIA(),orders_KIA,edit);
        if(dialog_KIA.ShowDialog(this)!=DialogResult.OK)return;
        if(edit)orders_KIA[orders_KIA.IndexOf(selected_KIA)]=dialog_KIA.Order;else orders_KIA.Add(dialog_KIA.Order);dirty_KIA=true;RefreshOrders();
    }
    private void DeleteOrder(){var selected_KIA=Selected();if(selected_KIA==null)return;if(MessageBox.Show(this,$"Удалить заказ {selected_KIA.Number}?","Удаление заказа",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;orders_KIA.Remove(selected_KIA);dirty_KIA=true;RefreshOrders();}
    private bool ConfirmUnsaved(){if(!dirty_KIA)return true;var answer_KIA=MessageBox.Show(this,"Сохранить изменения перед продолжением?","Несохранённые изменения",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);return answer_KIA==DialogResult.No||(answer_KIA==DialogResult.Yes&&SaveFile(false));}
    private void ResetFilters(){textBoxSearch_KIA.Clear();comboBoxStatus_KIA.SelectedIndex=0;checkBoxDateFilter_KIA.Checked=false;comboBoxSort_KIA.SelectedIndex=0;}
    private void NewFile(){if(!ConfirmUnsaved())return;orders_KIA=new();filePath_KIA=null;dirty_KIA=false;ResetFilters();RefreshOrders();}
    private void OpenExample(){if(!ConfirmUnsaved())return;LoadOrders(Path.Combine(AppContext.BaseDirectory,"Data","Orders.csv"),true);}
    private void OpenFile(){if(!ConfirmUnsaved())return;using var dialog_KIA=new OpenFileDialog{Filter="База заказов CSV (*.csv)|*.csv"};if(dialog_KIA.ShowDialog(this)==DialogResult.OK)LoadOrders(dialog_KIA.FileName,false);}
    private void LoadOrders(string path,bool example){try{var loaded_KIA=repository_KIA.Load(path);orders_KIA=loaded_KIA;filePath_KIA=example?null:path;dirty_KIA=example;ResetFilters();RefreshOrders();}catch(Exception ex) when(ex is IOException||ex is UnauthorizedAccessException||ex is FormatException){MessageBox.Show(this,ex.Message,"Не удалось открыть CSV",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    private bool SaveFile(bool saveAs){string path_KIA=filePath_KIA;if(saveAs||path_KIA==null){using var dialog_KIA=new SaveFileDialog{Filter="База заказов CSV (*.csv)|*.csv",FileName="Orders.csv"};if(dialog_KIA.ShowDialog(this)!=DialogResult.OK)return false;path_KIA=dialog_KIA.FileName;}try{repository_KIA.Save(path_KIA,orders_KIA);filePath_KIA=path_KIA;dirty_KIA=false;RefreshOrders();return true;}catch(Exception ex) when(ex is IOException||ex is UnauthorizedAccessException){MessageBox.Show(this,ex.Message,"Ошибка сохранения");return false;}}
    private void Export(){using var dialog_KIA=new SaveFileDialog{Filter="CSV (*.csv)|*.csv",FileName="Orders-filtered.csv"};if(dialog_KIA.ShowDialog(this)!=DialogResult.OK)return;if(!service_KIA.CanExportTo(dialog_KIA.FileName,filePath_KIA)){MessageBox.Show(this,"Выберите другой файл: экспорт выборки не должен перезаписывать открытую базу.","Экспорт выборки");return;}try{repository_KIA.Save(dialog_KIA.FileName,visible_KIA);MessageBox.Show(this,"Выборка сохранена.","Экспорт");}catch(Exception ex) when(ex is IOException||ex is UnauthorizedAccessException){MessageBox.Show(this,ex.Message,"Ошибка экспорта");}}
    private void ShowHelp(string text,string title)
    {using var dialog_KIA=new FormHelp_KIA(text,title);dialog_KIA.ShowDialog(this);}
    private void Guide()=>ShowHelp("1. Файл → Открыть CSV или Открыть пример.\n2. Добавить — заполните карточку заказа; * отмечены обязательные поля.\n3. Выберите строку и нажмите Изменить или дважды щёлкните по ней.\n4. Поиск, статус, даты и сортировка меняют текущую выборку.\n5. Статистика и диаграмма рассчитываются по видимым заказам, включая отменённые.\n6. Сохранить сохраняет всю базу, Экспорт — только выборку.\n7. CSV: UTF-8, разделитель ;, даты yyyy-MM-dd. Используйте пример.\n8. При закрытии программа предложит сохранить изменения.","Руководство пользователя — Заказы");
    private void About()=>ShowHelp("Учёт заказов\nКонев Илья Андреевич, НТм-25-1\nTyuiu.KonevIA.Sprint7.Project.V10\nТюменский индустриальный университет\nУчебный проект на C# / Windows Forms. Данные: CSV.","О программе");
}






internal sealed class BorderedToolbarRenderer_KIA : ToolStripProfessionalRenderer
{
    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        var bounds_KIA=new Rectangle(0,0,e.Item.Width-1,e.Item.Height-1);
        Color fill_KIA=!e.Item.Enabled?Color.FromArgb(242,244,247):e.Item.Pressed?Color.FromArgb(195,218,241):e.Item.Selected?Color.FromArgb(222,237,252):Color.White;
        using var brush_KIA=new SolidBrush(fill_KIA);using var pen_KIA=new Pen(Color.FromArgb(170,188,207));
        e.Graphics.FillRectangle(brush_KIA,bounds_KIA);e.Graphics.DrawRectangle(pen_KIA,bounds_KIA);
    }
}
