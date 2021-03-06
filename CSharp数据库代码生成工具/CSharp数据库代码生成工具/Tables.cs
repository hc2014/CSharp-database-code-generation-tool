﻿using System.Collections;
using System.IO;
using System.Xml;
using Common;
using Maunite.DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSharp数据库代码生成工具
{
    public partial class Tables : Form
    {
        public Tables()
        {
            InitializeComponent();
        }
        private string _strConn = "";
        public string StrDatabase,StrTableName="";
        public Dictionary<string, string> dic = new Dictionary<string, string>();
        public string strTableName
        {
            get
            {
                return StrTableName;
            }
        }
        public string strDatabase
        {
            get
            {
                return StrDatabase;
            }
        }
        
        private void Tables_Load(object sender, EventArgs e)
        {
            //Hide();
            listViewTables.GridLines = true;//显示各个记录的分隔线 
            listViewTables.FullRowSelect = true;//要选择就是一行 
            listViewTables.View = View.Details;//定义列表显示的方式 
            listViewTables.Scrollable = true;//需要时候显示滚动条 
            listViewTables.MultiSelect = false; // 不可以多行选择 
            listViewTables.HeaderStyle = ColumnHeaderStyle.Clickable;

           // tabPage2.Parent = tabPage1.Parent = null;

            labDataBase.Visible = false;
            comDataBase.Visible = false;

            label4.Text = @"如果在使用中有不明白的地方可以咨询作者。";
            label5.Text = @"作者:高增智";
            label6.Text = @"QQ:365238004 ";
            //linkLabel1.lin

            // StrDatabase = ((form11)this.Owner).comDataBaseText;
            // LoadData();

        } 
        

        //***********************得到数据集并绑定到listViewTables控件************/
        //清空listViewTables
        //设置表头。
        //执行数据库查询操作，得到表中所要显示的数据
        //数据按行绑定到eListView
        /*********************************************************************/
        private void LoadData()
        {
            this.listViewTables.Clear();

            // 针对数据库的字段名称，建立与之适应显示表头 
            listViewTables.Columns.Add("序号", 50, HorizontalAlignment.Left);
            listViewTables.Columns.Add("表名", 180, HorizontalAlignment.Left);
            listViewTables.Columns.Add("表说明", 120, HorizontalAlignment.Left);
            listViewTables.Columns.Add("数据总数", 80, HorizontalAlignment.Left);
            listViewTables.Visible = true;

            // 针对数据库的字段名称，建立与之适应显示表头  避免没选择表时左侧为空
            listViewColumns.Columns.Add("序号", 50, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段名", 120, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段类型", 80, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段说明", 120, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("数据总数", 80, HorizontalAlignment.Left);
            listViewColumns.Visible = true;

            //var sql = " select top 1000" +
            //                   " a.name AS 表名 ," +
            //                   " isnull(g.[value],'-') AS 说明" +
            //                   " from" +
            //                   " sys.tables a left join sys.extended_properties g " +
            //                   "on (a.object_id = g.major_id AND g.minor_id = 0) order by 表名 asc";

            var sql = "SELECT a.name AS 表名, isnull(d.[value],'-') AS 说明, b.rows as 总数"+
                       " FROM sysobjects a WITH(NOLOCK)" +
                       " JOIN sysindexes b WITH(NOLOCK)" +
                       " ON b.id = a.id" +
                       " join sys.tables t" +
                       " on t.object_id=a.id" +
                       " left join sys.extended_properties d" +
                       " on (t.object_id = d.major_id AND d.minor_id = 0)" +
                       " WHERE a.xtype = 'U ' AND b.indid IN (0, 1)" +
                       " ORDER By a.name ASC";

            ListViewHelper.DisplayDataSet(listViewTables, GetDataSet(sql), true);
            //int i = 0;
            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow row in ds.Tables[0].Rows)
            //    {
            //        i++;
            //        var item = new ListViewItem();
            //        item.SubItems.Clear();
            //        item.SubItems[0].Text = i.ToString();
            //        item.SubItems.Add(row["表名"].ToString());
            //        item.SubItems.Add(row["说明"].ToString());
            //        listViewTables.Items.Add(item);
            //    }


            //}

        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            try
            {

                DataSet ds = DbHelperSQL.Query("use [" + StrDatabase + "];" + sql);
                Show();
                return ds;
            }
            catch (Exception ex)
            {
                this.Close();
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        private void listViewTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTables.SelectedItems.Count > 0)
            {
               
                //MessageBox.Show(listViewTables.SelectedItems[0].SubItems[1].Text+@"-"+listViewTables.SelectedItems[0].SubItems[2].Text);
                StrTableName = listViewTables.SelectedItems[0].SubItems[1].Text;
                
                LoadTableColumns(StrTableName);

                Clipboard.SetDataObject(StrTableName); //则把数据置于剪切板中

                for (int i = 0; i < listViewTables.Items.Count; i++)
                {
                    if (listViewTables.Items[i].Selected == true)
                    {
                        listViewTables.SelectedItems[0].BackColor = Color.DodgerBlue;
                        listViewTables.SelectedItems[0].ForeColor = Color.White;
                    }
                    else
                    {
                        listViewTables.Items[i].BackColor = Color.White;
                        listViewTables.Items[i].ForeColor = Color.Black;
                    }

                }
            }
        }
        /// <summary>
        /// 绑定表字段
        /// </summary>
        private void LoadTableColumns(string tableName)
        {
            this.listViewColumns.Clear();

            // 针对数据库的字段名称，建立与之适应显示表头 
            listViewColumns.Columns.Add("序号", 50, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段名", 120, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段类型", 80, HorizontalAlignment.Left);
            listViewColumns.Columns.Add("字段说明", 150, HorizontalAlignment.Left);
            listViewColumns.Visible = true;
           

            var sql = new StringBuilder();

            sql.AppendLine("select c.name as columnName,t.name as columnType,p.value as columnDescription   from  sysobjects o left join syscolumns c  on o.id=c.id");
            sql.AppendLine(" left join sys.extended_properties p on p.major_id=c.id and p.minor_id=c.colid and p.name='MS_Description' left join systypes t on c.xusertype=t.xusertype");
            sql.AppendLine("where o.type='u' ");
            sql.AppendLine("and o.name='" + tableName + "'");

            ListViewHelper.DisplayDataSet(listViewColumns, GetDataSet(sql.ToString()), true);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(StrDatabase))
                {
                    MessageBox.Show("请先连接数据库！");
                    return;
                }
                //MessageBox.Show(StrTableName);
                richTemplate.Text = "model.字段名 = txt字段名.Text;//字段说明";

                listViewTemplate.GridLines = true;//显示各个记录的分隔线 
                listViewTemplate.FullRowSelect = true;//要选择就是一行 
                listViewTemplate.View = View.Details;//定义列表显示的方式 
                listViewTemplate.Scrollable = true;//需要时候显示滚动条 
                listViewTemplate.MultiSelect = false; // 不可以多行选择 
                listViewTemplate.HeaderStyle = ColumnHeaderStyle.Clickable;

                // 针对数据库的字段名称，建立与之适应显示表头
                listViewTemplate.Clear();
                listViewTemplate.Columns.Add("序号", 50, HorizontalAlignment.Left);
                listViewTemplate.Columns.Add("模板", 100, HorizontalAlignment.Left);
                listViewTemplate.Visible = true;
                if (dic.Count==0)
                {
                    dic.Add("Add", "model.字段名 = txt字段名.Text;//字段说明");
                    dic.Add("Edit", "txt字段名.Text= model.字段名;//字段说明");
                    dic.Add("asp:TextBox", "<asp:TextBox id=\"txt字段名\" runat=\"server\" />");
                    dic.Add("HtmlInput", "<input type=\"text\" id=\"txt字段名\" name=\"txt字段名\" />");
                    dic.Add("EasyUIColumns", "{ field:'字段名',title:'字段说明',width:100 },"); 
                }
                int i = 0;
                foreach (var dicItem in dic)
                {
                    i++;
                    var item = new ListViewItem();
                    item.SubItems.Clear();
                    item.SubItems[0].Text = i.ToString();
                    item.SubItems.Add(dicItem.Key);
                    listViewTemplate.Items.Add(item);
                }

                labSelectTableName.Text = StrTableName;
            }else if (tabControl1.SelectedIndex == 2)
            {
                if (string.IsNullOrEmpty(StrDatabase))
                {
                    MessageBox.Show("请先连接数据库！");
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (StrTableName=="")
            {
                MessageBox.Show("请选择要操作的表");
                return;
            }
            var sql = new StringBuilder();

            sql.AppendLine("select c.name as columnName,t.name as columnType,p.value as columnDescription   from  sysobjects o left join syscolumns c  on o.id=c.id");
            sql.AppendLine(" left join sys.extended_properties p on p.major_id=c.id and p.minor_id=c.colid and p.name='MS_Description' left join systypes t on c.xusertype=t.xusertype");
            sql.AppendLine("where o.type='u' ");
            sql.AppendLine("and o.name='" + StrTableName + "'");

            var ds = GetDataSet(sql.ToString());
            if (ds!=null&&ds.Tables[0].Rows.Count>0)
            {
                var result = new StringBuilder();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var temp = richTemplate.Text;
                    result.AppendLine(temp.Replace("字段名", row["columnName"].ToString()).Replace("字段说明", row["columnDescription"].ToString()));
                }
                richResult.Text = result.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(button2.Text); //则把数据置于剪切板中
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(button3.Text); //则把数据置于剪切板中
        }

        private void listViewTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTemplate.SelectedItems.Count > 0)
            {
                //MessageBox.Show(listViewTemplate.SelectedItems[0].SubItems[1].Text);
                richTemplate.Text = dic[listViewTemplate.SelectedItems[0].SubItems[1].Text];

            }
        }

        private void listViewColumns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (listViewColumns.SelectedItems.Count > 0)
                {
                    //将复制的内容放入剪切板中
                    if (listViewColumns.SelectedItems[0].Text != "")
                        Clipboard.SetDataObject(listViewColumns.SelectedItems[0].SubItems[1].Text +" "+ listViewColumns.SelectedItems[0].SubItems[3].Text);
                }
            }
        }

        private void Tables_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定要关闭吗！", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                e.Cancel = false;  //点击OK 
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }    
        }

        private void listViewColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewColumns.SelectedItems.Count > 0)
            {
                Clipboard.SetDataObject(textBox1.Text.Trim() + listViewColumns.SelectedItems[0].SubItems[1].Text);
            }
        }
     
        private void listViewTables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listViewTables.HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                if (int.Parse(info.Item.SubItems[3].Text)>0)
                {
                    StrTableName = info.Item.SubItems[1].Text;
                    //MessageBox.Show(StrTableName);
                    ShowDataForm table = new ShowDataForm();
                    table.Owner = this;
                    table.ShowDialog();
                }
                else
                {
                    MessageBox.Show("此表木有数据！");
                }
            } 
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            if (txtUser.Text.Trim() != "" && txtPwd.Text.Trim() != "" && txtServerUrl.Text.Trim() != "")
            {
                try
                {
                    _strConn = "server=" + txtServerUrl.Text.Trim() + ";uid=" + txtUser.Text.Trim() + ";pwd=" + txtPwd.Text.Trim() + ";database=master";

                    DbHelperSQL.connectionString = _strConn;
                    BindDatabase();
                    btnConnection.Enabled = false;
                    btnConfirm.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    btnConnection.Enabled = true;
                    btnConfirm.Enabled = false;
                }

            }
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public void BindDatabase()
        {

            var ds = DbHelperSQL.Query("select * from [sysdatabases] order by [name]");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                comDataBase.DataSource = ds.Tables[0];    //将表绑定到控件
                comDataBase.DisplayMember = "name";     //定义要显示的内容为列名为x的内容
                comDataBase.ValueMember = "dbid";       //定义要映射的值为y的值

                labDataBase.Visible = true;
                comDataBase.Visible = true;

            }



        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {

            //tabPage2.Parent = tabPage1.Parent = tabControl1;

            StrDatabase = comDataBase.Text;
             LoadData();
            tabControl1.SelectTab(1);
        }

       
        private void BtnChange_Click_1(object sender, EventArgs e)
        {
            this.txtbBottom.Text = changetext(this.txtbTop.Text);
        }
       
        private string changetext(string strText)
        {
            StringBuilder builder = new StringBuilder();
            string[] strcom = strText.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            char[] cha = { '=', ';' };
            string[] team;
            foreach (string item in strcom)
            {
                if (item.Contains("=") && item.Contains(";"))
                {
                    team = item.Trim().Split(cha);

                    builder.Append(Environment.NewLine);
                    builder.Append(subRep(team[1]) + "=" + team[0] + ";" + team[2]);
                    builder.Append(Environment.NewLine);
                }
                else
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(item);
                    builder.Append(Environment.NewLine);
                }
            }
            return builder.ToString();
        }

        private string subRep(string strt)
        {
            if (strt.IndexOf(".Trim()") == -1)
            {
                return strt;
            }
            else
            {
                return strt.Substring(0, strt.IndexOf(".Trim()"));
            }
        }

        private string Convert0ToCSharp(string strs, string _SourceCode)
        {
            if (string.IsNullOrEmpty(strs))
            {
                strs = "sb";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("<%");
            builder.Append(Environment.NewLine);
            builder.Append("StringBuilder " + strs + " = new StringBuilder();");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            string[] strArray = _SourceCode.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Replace(@"\", @"\\").Replace("\"", "\\\"");
                builder.AppendFormat("" + strs + ".AppendLine(\"{0}\");", str);
                builder.Append(Environment.NewLine);
            }
            builder.Append(Environment.NewLine);
            builder.Append("return " + strs + ".ToString();");
            builder.Append(Environment.NewLine);
            builder.Append("%>");
            return builder.ToString();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.txtbBottom.Text = Convert0ToCSharp(this.TxtbString.Text.Trim(), this.txtbTop.Text);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.txtbTop.Text = "";
            this.txtbBottom.Text = "";
        }

        private void btnaddfile_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.FileName.IndexOf(".") > 0)
            {
                this.txtformatxml.Text = FormatXmlFile(this.openFileDialog1.FileName);
            }
            else
            {
                this.txtformatxml.Text = FormatXmlString(this.txtformatxmlsource.Text.Trim());
            }
        }


        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="AString"></param>
        /// <returns></returns>
        public static string FormatXmlString(string AString)
        {
            string str2;
            try
            {
                MemoryStream w = new MemoryStream(0x400);
                XmlTextWriter writer = new XmlTextWriter(w, null);
                XmlDocument document = new XmlDocument();
                writer.Formatting = Formatting.Indented;
                document.LoadXml(AString);
                document.WriteTo(writer);
                writer.Flush();
                writer.Close();
                str2 = Encoding.GetEncoding("utf-8").GetString(w.ToArray());
                w.Close();
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                //Exception exception = exception1;
                str2 = "";
                //Interaction.MsgBox(exception.Message, MsgBoxStyle.OkOnly, null);
                //ProjectData.ClearProjectError();
            }
            return str2;
        }

        /// <summary>
        /// 格式化文件
        /// </summary>
        /// <param name="AFile"></param>
        /// <returns></returns>
        public static string FormatXmlFile(string AFile)
        {
            string str2;
            try
            {
                MemoryStream w = new MemoryStream(0x400);
                XmlTextWriter writer = new XmlTextWriter(w, null);
                XmlDocument document = new XmlDocument();
                writer.Formatting = Formatting.Indented;
                document.Load(AFile);
                document.WriteTo(writer);
                writer.Flush();
                writer.Close();
                str2 = Encoding.GetEncoding("utf-8").GetString(w.ToArray());
                w.Close();
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                //Exception exception = exception1;
                str2 = "";
                //Interaction.MsgBox(exception.Message, MsgBoxStyle.OkOnly, null);
                //ProjectData.ClearProjectError();
            }
            return str2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.txtformatxml.Text = "";
            this.txtformatxmlsource.Text = "";
        }

        private void btnsavefile_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //this.Save_Path_Text.Text = this.CheckFileJ.SelectedPath + @"\";
                    File.WriteAllText(this.saveFileDialog1.FileName + "_Format.xml", txtformatxml.Text, Encoding.UTF8);

                    MessageBox.Show("保存成功！");
                }
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                //Exception exception = exception1;
                //Interaction.MsgBox(exception.Message, MsgBoxStyle.OkOnly, null);
                //ProjectData.ClearProjectError();
            }
        }

       



    }
}
