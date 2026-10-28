using Hydra;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using OtworzOkres;
using System.Collections.Generic;
using static OtworzOkres.ZS;
using System.Text;
using System;
using System.Data.SqlClient;
using System.Data;
using cdn_api;

//do update: WHERE Okr_id=(SELECT MAX(OKR_ID) FROM[ERPXL_RainbowTours].[CDN].[Okresy] WHERE OKR_ZamknietyDo<>0)

[assembly: CallbackAssemblyDescription(Name: "ONE-014 Otwieranie okresów",
                                           Description: "Otwieranie okresów w Księgowości",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2021.0.2",
                                           Created: "17-10-2022")]


namespace OtworzOkres
{
    [SubscribeProcedure(Procedures.WybierzOkres, "Otwieranie okresów")]
    public class ZSnaFS_Class : Callback
    {
        
        private ClaWindow RapButton;
        private ClaWindow zakladkaTabWewnetrzne;
        private ClaWindow filtrBtn;
        private ClaWindow listaPoz;
        
        public override void Init()
        {

            AddSubscription(true, 0, Events.JustAfterWindowOpening, new TakeEventDelegate(OnOpenWindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
        }



        private bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {

            ClaWindow Parent = GetWindow();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Rectangle WndBnd = GetWindow().Bounds;
            zakladkaTabWewnetrzne = GetWindow().AllChildren["?List"];
            //listaPoz = GetWindow().AllChildren["?ListaZam"];
            //RapButton = zakladkaTabWewnetrzne.Children.Add(ControlTypes.button);
            RapButton = Parent.Children.Add(ControlTypes.button);
            RapButton.Visible = true;
            RapButton.TextRaw = "Open";
            RapButton.ToolTipRaw = "Otwieranie ostatniego zamkniętego okresu";
            //RapButton.IconRaw = "C:\\OneSupport\\klodka.ico";

            filtrBtn = GetWindow().AllChildren["?Select"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 36, filtrBtn.Bounds.Y, 30, 15);
            //listID = GetWindow().AllChildren["?ListaZam"].Id;      //tu sprawdzic


            //RapButton.OnBeforeAccepted += new TakeEventDelegate(zaczytaj_OnAfterAccepted);
            RapButton.OnAfterMouseDown += new TakeEventDelegate(BtnOnMouseDown);
            //pobrane z pola w zakładce Raporty
            return true;
        }

        public bool Resize(Procedures ProcId, int ControlId, Events Event)
        {
            filtrBtn = GetWindow().AllChildren["?Select"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 36, filtrBtn.Bounds.Y, 30, 15);

            return true;
        }

        
        public bool BtnOnMouseDown(Procedures ProcId, int ControlId, Events Event)
        {

            try
            {


                
                string connectionString = "";
                if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 25) == "ERPXL_RainbowTours_TESTRT")
                    connectionString = @"Data Source = XL; Initial Catalog = ERPXL_RainbowTours_TESTRT; User ID = onesupport; Password=g54q@8w#v$Y.Df_8f4hg";
                else if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 18) == "ERPXL_RainbowTours")
                    connectionString = @"Data Source = XL; Initial Catalog = ERPXL_RainbowTours; User ID = onesupport; Password=g54q@8w#v$Y.Df_8f4hg";
                

                string updateOkres = "UPDATE cdn.Okresy set OKR_ZamknietyDo=0 WHERE Okr_id=(SELECT MAX(OKR_ID) FROM [CDN].[Okresy] WHERE OKR_ZamknietyDo<>0)";
                Update(updateOkres, connectionString);

              
                MessageBox.Show("Otwarto ostatni zamknięty okres", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch(Exception e)
            {
                MessageBox.Show("Błąd otwierania okresu: "+e.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            
            return true;
        }


        

       

        public string Update(string sql, string connection_string)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.UpdateCommand = new SqlCommand(sql, connection);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row updated !! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }

        


        
        public override void Cleanup()
        {

        }

    }
}
