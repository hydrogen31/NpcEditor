using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace NpcEditor
{
    public partial class Form1 : Form
    {
        private NpcInfo SelectedNPC;
        public Form1()
        {
            InitializeComponent();
            ChangeLanguage(CultureInfo.CurrentCulture.Name);
        }

        // Form Load Event
        private void Form1_Load(object sender, EventArgs e)
        {
            DbMgr.Init();
            LoadSwf();
            ReloadNpcs();
            LoadMapItems();
            textBoxResource.Text = ConfigurationManager.AppSettings["ResourceAddress"];
        }

        // Button Click Events
        private void button1_Click(object sender, EventArgs e)
        {
            if (SelectedNPC != null)
            {
                string swfFile = textBoxResource.Text + "/" + SelectedNPC.ResourcesPath;
                string frameLabel = textBoxActionMovie.Text;
                string spriteName = textBoxModelID.Text;
                SendFlashCall("loadAndPlay", swfFile, spriteName, frameLabel,
                    SelectedNPC.X, SelectedNPC.Y, SelectedNPC.Width, SelectedNPC.Height);
            }
            else
            {
                MessageBox.Show(LanguageMgr.GetTranslation("NpcNotSelected"));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadSwf();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateNpcHitbox();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadHitboxFromNpcInfo(SelectedNPC);
            SendDrawHitboxFromNpcInfo();
        }

        // ListBox SelectedIndexChanged Events
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                ListBox.SelectedObjectCollection selectedItems = listBox1.SelectedItems;

                SelectedNPC = (NpcInfo)selectedItems[0];
                textBoxModelID.Text = SelectedNPC.ModelID;
                textBoxSelectedNpcID.Text = SelectedNPC.ID.ToString();
                textBoxSelectedNpcScript.Text = SelectedNPC.Script;
                LoadHitboxFromNpcInfo(SelectedNPC);
                button1_Click(null, null);


                if (selectedItems.Count > 1) // Eğer birden fazla öğe seçiliyse ModelID'si aynı olmayanları seçtirmeyeceğim
                {
                    string firstSelectedModelID = SelectedNPC.ModelID;
                    listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                    {
                        NpcInfo selectedItem = (NpcInfo)selectedItems[i];

                        // Eğer seçili öğenin ModelID'si ilk seçili öğenin ModelID'si ile aynı değilse, seçimden kaldır
                        if (selectedItem.ModelID != firstSelectedModelID)
                        {
                            listBox1.SetSelected(listBox1.Items.IndexOf(selectedItem), false);
                        }
                    }
                    listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
                }
            }
            else
            {
                textBoxSelectedNpcID.Text = LanguageMgr.GetTranslation("textBoxSelectedNpcID.Text");
                textBoxSelectedNpcScript.Text = "";
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                textBoxActionMovie.Text = (string)listBox2.SelectedItem;
                SendPlayMovie(textBoxActionMovie.Text);
            }
        }

        // Text Changed Events
        private void NpcSearch_TextChanged(object sender, EventArgs e)
        {
            ReloadNpcs();
        }

        private void hitbox_TextChanged(object sender, EventArgs e)
        {
            SendDrawHitboxFromTextBoxes();
            UpdateHitboxButtons();
        }

        // ComboBox SelectedIndexChanged Event
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeLanguage(comboBox1.SelectedIndex == 1 ? "tr" : CultureInfo.InvariantCulture.Name);
        }

        // Flash Call Event
        private void axShockwaveFlash1_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            string xmlContent = e.request;
            //Console.WriteLine(xmlContent);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            try
            {
                string functionName = xmlDoc.SelectSingleNode("/invoke/@name").Value;

                XmlNodeList argumentNodes = xmlDoc.SelectNodes("/invoke/arguments/*");

                List<string> parameters = new List<string>();

                foreach (XmlNode argumentNode in argumentNodes)
                {
                    parameters.Add(argumentNode.InnerText);
                }

                // fonksiyonun adı belli, parameteleri belli. şimdi işleyelim
                switch (functionName)
                {
                    case "sendLabels":
                        HandleSendLabels(parameters);
                        break;
                    case "newHitbox":
                        HandleNewHitbox(parameters);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageMgr.GetTranslation("FlashCallError") + ex.ToString());
            }
        }

        // Helper Methods
        private void LoadSwf()
        {
            string swffile = "NpcLoader.swf";
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = Path.Combine(appDir, swffile);

            if (File.Exists(swffile))
            {
                axShockwaveFlash1.LoadMovie(0, "blank.swf");
                axShockwaveFlash1.Play();
                axShockwaveFlash1.Movie = filePath;
                axShockwaveFlash1.Play();
            }
            else
            {
                MessageBox.Show(LanguageMgr.GetTranslation("LoaderSwfNotFound", swffile));
            }
        }

        private void ReloadNpcs()
        {
            IEnumerable<NpcInfo> npcs = DbMgr.NPC_List;

            string name = textBoxSearchNpcName.Text.ToLower();
            string npcId = textBoxSearchNpcID.Text.ToLower();

            if (!string.IsNullOrEmpty(name))
            {
                npcs = npcs.Where(x => x.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrEmpty(npcId))
            {
                npcs = npcs.Where(x => x.ID.ToString().ToLower().Contains(npcId));
            }

            listBox1.DataSource = npcs.ToList();
            listBox1.DisplayMember = "Name";
        }

        private void LoadMapItems()
        {
            comboBoxMap.DataSource = DbMgr.Map_List;
            comboBoxMap.DisplayMember = "DisplayName";
        }

        private void ChangeLanguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            foreach (Control control in this.Controls)
            {
                UpdateControlTexts(control);
            }
        }

        private void UpdateControlTexts(Control control)
        {
            if (control.Tag != null)
            {
                string tag = control.Tag.ToString();
                if (control is Label label)
                {
                    label.Text = LanguageMgr.GetTranslation(tag);
                }
                else if (control is Button button)
                {
                    button.Text = LanguageMgr.GetTranslation(tag);
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.Text = LanguageMgr.GetTranslation(tag);
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Text = LanguageMgr.GetTranslation(tag);
                }
            }
            //güncellenmesi gereken başka kontroller varsa onları da eklersin
            else if (control.HasChildren)
            {
                foreach (Control childControl in control.Controls)
                {
                    UpdateControlTexts(childControl);
                }
            }
        }

        private void SendFlashCall(string name, params object[] args)
        {
            if (axShockwaveFlash1.Movie != null)
                axShockwaveFlash1.CallFunction("<invoke name=\"handleFlashCall\" returntype=\"xml\"><arguments><string>" + name + "</string><string>" + string.Join(",", args) + "</string></arguments></invoke>");
        }

        private void SendPlayMovie(string actionName)
        {
            SendFlashCall("playMovie", actionName);
        }

        private void SendDrawHitboxFromNpcInfo()
        {
            SendFlashCall("drawHitbox", SelectedNPC.X, SelectedNPC.Y, SelectedNPC.Width, SelectedNPC.Height);
        }

        private void SendDrawHitboxFromTextBoxes()
        {
            if (int.TryParse(numericUpDownNpcX.Text, out int x) &&
                int.TryParse(numericUpDownNpcY.Text, out int y) &&
                int.TryParse(numericUpDownNpcWidth.Text, out int width) &&
                int.TryParse(numericUpDownNpcHeight.Text, out int height))
            {
                SendFlashCall("drawHitbox", x, y, width, height);
            }
        }

        private void LoadHitboxFromNpcInfo(NpcInfo npc)
        {
            numericUpDownNpcX.Text = npc.X.ToString();
            numericUpDownNpcY.Text = npc.Y.ToString();
            numericUpDownNpcWidth.Text = npc.Width.ToString();
            numericUpDownNpcHeight.Text = npc.Height.ToString();
        }

        private void UpdateNpcHitbox()
        {
            if (SelectedNPC == null)
            {
                MessageBox.Show(LanguageMgr.GetTranslation("NpcNotSelected"));
                return;
            }

            if (int.TryParse(numericUpDownNpcX.Text, out int x) &&
                int.TryParse(numericUpDownNpcY.Text, out int y) &&
                int.TryParse(numericUpDownNpcWidth.Text, out int width) &&
                int.TryParse(numericUpDownNpcHeight.Text, out int height))
            {
                List<NpcInfo> npcs = new List<NpcInfo>();
                for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                {
                    NpcInfo npc = (NpcInfo)listBox1.SelectedItems[i];
                    npc.SetHitbox(x, y, width, height);

                    if (DbMgr.UpdateNpcHitbox(npc))
                    {
                        //MessageBox.Show(LanguageMgr.GetTranslation("NpcHitboxUpdated"));
                        UpdateHitboxButtons();
                    }
                    else
                    {
                        MessageBox.Show(LanguageMgr.GetTranslation("NpcHitboxUpdateError"));
                    }
                }
            }
            else
            {
                MessageBox.Show(LanguageMgr.GetTranslation("NpcHitboxInvalidData"));
            }
        }

        private bool HitboxIsDirty()
        {
            if (int.TryParse(numericUpDownNpcX.Text, out int x) &&
               int.TryParse(numericUpDownNpcY.Text, out int y) &&
               int.TryParse(numericUpDownNpcWidth.Text, out int width) &&
               int.TryParse(numericUpDownNpcHeight.Text, out int height))
            {
                if (SelectedNPC.X != x ||
                SelectedNPC.Y != y ||
                SelectedNPC.Width != width ||
                SelectedNPC.Height != height)
                    return true;

            }
            return false;
        }

        private void UpdateHitboxButtons()
        {
            bool isDirty = HitboxIsDirty();
            if (isDirty && !buttonHitboxSave.Enabled)
            {
                buttonHitboxSave.Enabled = true;
                buttonHitboxReset.Enabled = true;
            }
            else if (!isDirty && buttonHitboxSave.Enabled)
            {
                buttonHitboxSave.Enabled = false;
                buttonHitboxReset.Enabled = false;
            }
        }
        private void HandleSendLabels(List<string> parameters)
        {
            string[] labelArray = parameters[0].Split(',');
            listBox2.DataSource = labelArray;
        }

        private void HandleNewHitbox(List<string> parameters)
        {
            numericUpDownNpcX.Text = parameters[0];
            numericUpDownNpcY.Text = parameters[1];
            numericUpDownNpcWidth.Text = parameters[2];
            numericUpDownNpcHeight.Text = parameters[3];
        }

        private void checkBoxMulti_CheckedChanged(object sender, EventArgs e)
        {
            bool needUpdate = listBox1.SelectedItems.Count > 1 && SelectedNPC != null && listBox1.Items.Contains(SelectedNPC);
            listBox1.SelectionMode = checkBoxMulti.Checked ? SelectionMode.MultiExtended : SelectionMode.One;
            if (needUpdate)
                listBox1.SelectedItem = SelectedNPC;
        }

        private void buttonLoadMap_Click(object sender, EventArgs e)
        {
            if(comboBoxMap.SelectedItem == null)
            {
                MessageBox.Show(LanguageMgr.GetTranslation("MapNotSelected"));
                return;
            }

            MapInfo map = (MapInfo)comboBoxMap.SelectedItem;
            if (map != null)
            {
                string deadUrl = string.IsNullOrEmpty(map.DeadPic) ? "" : textBoxResource.Text + "/image/map/" + map.ID + "/" + map.DeadPic + ".png";
                string foreUrl = string.IsNullOrEmpty(map.ForePic) ? "" : textBoxResource.Text + "/image/map/" + map.ID + "/" + map.ForePic + ".png";
                
                string posX = map.PosX.Replace(',', '-');
                string posX1 = map.PosX1.Replace(',', '-');
                
                SendFlashCall("setMap", deadUrl, foreUrl, posX, posX1);
            }
            else
            {
                MessageBox.Show(LanguageMgr.GetTranslation("MapNotFound"));
            }

        }
    }
}
