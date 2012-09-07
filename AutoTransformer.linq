const string xsltLocation = "c:\\temp\\myXslt.xsl";
const string xmlFile = "c:\\temp\\xslData.xml";
const string outputPath = "c:\\temp\\output.xml";
const string outputApp = "winword";

void Main()
{
	//Create file watcher
	var watcher = new FileSystemWatcher();
	watcher.Path = Path.GetDirectoryName(xsltLocation);
	watcher.NotifyFilter = NotifyFilters.LastWrite;
	watcher.Filter = Path.GetFileName(xsltLocation);
	watcher.Changed += new FileSystemEventHandler(OnChanged);
	watcher.EnableRaisingEvents = true;
	//Wait until infinity is here
	Thread.Sleep(Timeout.Infinite);	
}

void OnChanged(object source, FileSystemEventArgs e)
{
	var processes = System.Diagnostics.Process.GetProcessesByName(outputApp);
	var openWindow = (from p in processes where p.MainWindowTitle.ToUpper().Contains(Path.GetFileName(outputPath.ToUpper())) select p).FirstOrDefault();	
	if(openWindow!=null)	
		openWindow.Kill();	
	var attempts = 0;		
	while(attempts<3)
	{
		try{
			var myXslTransform = new XslTransform();
			myXslTransform.Load(xsltLocation); 			
			myXslTransform.Transform(xmlFile, outputPath); 
			System.Diagnostics.Process.Start(outputApp,outputPath);	
			break;
		}
		catch{
			//Sometimes takes word a few cycles to release the output file so we
			//try a couple of times before giving up
			Thread.Sleep(50);
			attempts++;
		}
	}	
}
