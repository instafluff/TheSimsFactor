using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheSimsFactor
{
	public class SimsInfo
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Career { get; set; }
		public string Serialize()
		{
			return string.Format( "{0};{1};{2};{3}", this.FirstName, this.LastName, this.Gender, this.Career );
		}
		public static SimsInfo Deserialize( string val )
		{
			try
			{
				var lines = val.Split( ';' );
				return new SimsInfo() {
					FirstName = lines[ 0 ],
					LastName = lines[ 1 ],
					Gender = lines[ 2 ],
					Career = lines[ 3 ]
				};
			}
			catch
			{
				return null;
			}
		}
	}

	public partial class Form1 : Form
	{
		List<SimsInfo> Sims = new List<SimsInfo>();

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load( object sender, EventArgs e )
		{
			//if( !File.Exists( @"D:\Program Files (x86)\Origin Games\The Sims 4\Game\Bin\simsfactor.txt" ) ) {
			//	File.WriteAllText( @"D:\Program Files (x86)\Origin Games\The Sims 4\Game\Bin\simsfactor.txt", "" );
			//}
			var watchMySim = new FileSystemWatcher( @"D:\Program Files (x86)\Origin Games\The Sims 4\Game\Bin\" );
			watchMySim.NotifyFilter = NotifyFilters.LastWrite;
			watchMySim.Filter = "*.comfy";
			watchMySim.EnableRaisingEvents = true;
			watchMySim.Changed += WatchMySim_Changed;
			label1.Text = "TESTTESTTEST";
		}

		private void WatchMySim_Changed( object sender, FileSystemEventArgs e )
		{
			if( WaitForFile( e.FullPath ) )
			{
				var lines = File.ReadAllLines( e.FullPath );
				var version = lines[ 0 ];
				switch( e.Name )
				{
					case "simsfactor.comfy":
						{
							File.Delete( e.FullPath );
							Sims.Clear();
							for( var i = 1; i < lines.Length; i++ )
							{
								var info = lines[ i ].Split( ',' );
								Sims.Add( new SimsInfo() {
									FirstName = info[ 0 ],
									LastName = info[ 1 ],
									Gender = info[ 2 ],
									Career = info[ 3 ].Split( '_' ).Last()
								} );
							}
							using( var wb = new WebClient() )
							{
								var data = new NameValueCollection();
								var qs = string.Join( "|", Sims.Select( x => x.Serialize() ).ToArray() );

								var response = wb.UploadValues( "http://localhost:8090/simsfactor/vote?sims=" + qs, "POST", data );
								string responseInString = Encoding.UTF8.GetString( response );
							}
						}
						break;
					case "endvote.comfy":
						{
							File.Delete( e.FullPath );
							Sims.Clear();
							using( var wb = new WebClient() )
							{
								var response = wb.DownloadString( "http://localhost:8090/simsfactor/results" );
								Console.WriteLine( response );
							}
						}
						break;
				}
			}
		}

		private void exitToolStripMenuItem_Click( object sender, EventArgs e )
		{
			this.Close();
		}

		/// <summary>
		/// Blocks until the file is not locked any more.
		/// </summary>
		/// <param name="fullPath"></param>
		bool WaitForFile( string fullPath )
		{
			int numTries = 0;
			while( true )
			{
				++numTries;
				try
				{
					// Attempt to open the file exclusively.
					using( FileStream fs = new FileStream( fullPath,
						FileMode.Open, FileAccess.ReadWrite,
						FileShare.None, 100 ) )
					{
						fs.ReadByte();

						// If we got this far the file is ready
						break;
					}
				}
				catch( Exception ex )
				{
					if( numTries > 10 )
					{
						return false;
					}

					// Wait for the lock to be released
					System.Threading.Thread.Sleep( 500 );
				}
			}
			return true;
		}
	}
}
