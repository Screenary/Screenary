
// This file has been generated by the GUI designer. Do not modify.
namespace Screenary.Client
{
	public partial class ConnectDialog
	{
		private global::Gtk.VBox vbox3;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label lblHostname;
		private global::Gtk.Entry txtHostname;
		private global::Gtk.Label lblPort;
		private global::Gtk.Entry txtPort;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonConnect;
        
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Screenary.Client.ConnectDialog
			this.Name = "Screenary.Client.ConnectDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Connection");
			this.Icon = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Resizable = false;
			this.AllowGrow = false;
			// Internal child Screenary.Client.ConnectDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.Spacing = 6;
			w1.BorderWidth = ((uint)(6));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblHostname = new global::Gtk.Label ();
			this.lblHostname.Name = "lblHostname";
			this.lblHostname.LabelProp = global::Mono.Unix.Catalog.GetString ("Hostname:");
			this.hbox1.Add (this.lblHostname);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblHostname]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(6));
			// Container child hbox1.Gtk.Box+BoxChild
			this.txtHostname = new global::Gtk.Entry ();
			this.txtHostname.WidthRequest = 277;
			this.txtHostname.CanFocus = true;
			this.txtHostname.Name = "txtHostname";
			this.txtHostname.Text = global::Mono.Unix.Catalog.GetString ("localhost");
			this.txtHostname.IsEditable = true;
			this.txtHostname.InvisibleChar = '•';
			this.hbox1.Add (this.txtHostname);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.txtHostname]));
			w3.Position = 1;
			w3.Padding = ((uint)(6));
			// Container child hbox1.Gtk.Box+BoxChild
			this.lblPort = new global::Gtk.Label ();
			this.lblPort.Name = "lblPort";
			this.lblPort.LabelProp = global::Mono.Unix.Catalog.GetString ("Port:");
			this.hbox1.Add (this.lblPort);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lblPort]));
			w4.Position = 2;
			w4.Expand = false;
			w4.Fill = false;
			w4.Padding = ((uint)(6));
			// Container child hbox1.Gtk.Box+BoxChild
			this.txtPort = new global::Gtk.Entry ();
			this.txtPort.WidthRequest = 144;
			this.txtPort.CanFocus = true;
			this.txtPort.Name = "txtPort";
			this.txtPort.Text = global::Mono.Unix.Catalog.GetString ("4489");
			this.txtPort.IsEditable = true;
			this.txtPort.InvisibleChar = '•';
			this.hbox1.Add (this.txtPort);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.txtPort]));
			w5.Position = 3;
			w5.Padding = ((uint)(6));
			this.vbox3.Add (this.hbox1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			w1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox3]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			w7.Padding = ((uint)(6));
			// Internal child Screenary.Client.ConnectDialog.ActionArea
			global::Gtk.HButtonBox w8 = this.ActionArea;
			w8.Name = "dialog1_ActionArea";
			w8.Spacing = 10;
			w8.BorderWidth = ((uint)(5));
			w8.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w9 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this.buttonCancel]));
			w9.Expand = false;
			w9.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonConnect = new global::Gtk.Button ();
			this.buttonConnect.CanDefault = true;
			this.buttonConnect.CanFocus = true;
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.UseUnderline = true;
			// Container child buttonConnect.Gtk.Container+ContainerChild
			global::Gtk.Alignment w10 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w11 = new global::Gtk.HBox ();
			w11.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w12 = new global::Gtk.Image ();
			w12.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
			w11.Add (w12);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w14 = new global::Gtk.Label ();
			w14.LabelProp = global::Mono.Unix.Catalog.GetString ("Connect");
			w14.UseUnderline = true;
			w11.Add (w14);
			w10.Add (w11);
			this.buttonConnect.Add (w10);
			this.AddActionWidget (this.buttonConnect, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w18 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8 [this.buttonConnect]));
			w18.Position = 1;
			w18.Expand = false;
			w18.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 608;
			this.DefaultHeight = 134;
			this.Show ();
			this.buttonCancel.Clicked += new global::System.EventHandler (this.OnButtonCancelClicked);
			this.buttonConnect.Clicked += new global::System.EventHandler (this.OnButtonConnectClicked);
		}
	}
}
