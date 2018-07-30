using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;



namespace JSXML {
	

public class REXML {
	public string XML;
	public XMLElement rootElement;
	
	public REXML(string sXML) {
		this.XML = sXML;
		this.rootElement = null;
		this.parse();
	}

	public REXML() {
		this.XML = "";
		
		this.rootElement = null;

	}
	
	public bool Load(string sFileName) {
		if (!File.Exists(sFileName)) {
			ContribuHelp.Debug.Comment("File not found: " + sFileName);
			return false;
		}

        using (StreamReader sr = File.OpenText(sFileName)) {
        	string s = "";
            while ((s = sr.ReadLine()) != null) {
                this.XML += s;
            }
        	this.parse();
           return true;
        }
	}

	public bool Save(string sFileName) {
        using (FileStream fs = File.OpenWrite(sFileName)) {
            Byte[] xml = 
                new UTF8Encoding(true).GetBytes(this.XML);

            fs.SetLength(xml.GetLength(0));
            fs.Write(xml, 0, xml.Length);
        }
		return true;
	}

	public void parse() {
		Regex reTag = new Regex("<([^>/ ]*)([^>]*)>");		
		Regex reTagText = new Regex("<([^>/ ]*)([^>]*)>([^<]*)");		
		string strType = "";
		string strTag = "";
		string strText = "";
		string strAttributes = "";
		string strOpen = "";
		string strClose = "";
		int iElements = 0;
		XMLElement xmleLastElement = null;
		Match[] arrElementsUnparsed = new Match[512];
		Match[] arrElementsUnparsedText = new Match[512];
		
		reTag.Matches(this.XML).CopyTo(arrElementsUnparsed, 0);
		reTagText.Matches(this.XML).CopyTo(arrElementsUnparsedText, 0);

		int i = 0;
		if (reTag.Replace(arrElementsUnparsed[0].ToString(), "$1") == "?xml") i++;
		for (; i<Array.IndexOf(arrElementsUnparsed, null); i++) {

			strTag = reTag.Replace(arrElementsUnparsed[i].ToString(), "$1");
			//if (strTag.Length == 0) continue;
			strAttributes = reTag.Replace(arrElementsUnparsed[i].ToString(),"$2");
			strText = reTagText.Replace(arrElementsUnparsedText[i].ToString(), "$3");
			strText = new Regex("[\r\n\t ]+").Replace(strText, " "); // remove white space
			strClose = "";
			if (strTag.IndexOf("![CDATA[") == 0) {
				strOpen = "<![CDATA[";
				strClose = "]]>";
				strType = "cdata";
			} else if (strTag.IndexOf("!--") == 0) {
				strOpen = "<!--";
				strClose = "-->";
				strType = "comment";
			} else if (strTag.IndexOf("?") == 0) {
				strOpen = "<?";
				strClose = "?>";
				strType = "pi";
			} else strType = "element";
			if (strClose != "") {
				strText = "";
				if (arrElementsUnparsedText[i].ToString().IndexOf(strClose) > -1) strText = arrElementsUnparsedText[i].ToString();
				else {
					for (; i<Array.IndexOf(arrElementsUnparsed, null) && arrElementsUnparsedText[i].ToString().IndexOf(strClose) == -1; i++) {
						strText += arrElementsUnparsedText[i];
					}
					strText += arrElementsUnparsedText[i];
				}
				if (strText.Substring(strOpen.Length, strText.IndexOf(strClose)) != "")	{
					xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement(strType, "","",xmleLastElement,strText.Substring(strOpen.Length, strText.IndexOf(strClose)));
					if (strType == "cdata") xmleLastElement.text += strText.Substring(strOpen.Length, strText.IndexOf(strClose));
				}
				if (strText.IndexOf(strClose)+ strClose.Length < strText.Length) {
					xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement("text", "","",xmleLastElement,strText.Substring(strText.IndexOf(strClose)+ strClose.Length, strText.Length));
					if (strType == "cdata") xmleLastElement.text += strText.Substring(strText.IndexOf(strClose)+ strClose.Length, strText.Length);
				}
				continue;
			}
			if ((new Regex(" *")).Replace(strText , "") == "") strText = "";
			if (arrElementsUnparsed[i].ToString().Substring(1,1) != "/") {
				if (iElements == 0) {
					xmleLastElement = this.rootElement = new XMLElement(strType, strTag,strAttributes,null,strText);
					iElements++;
					if (strText != "") xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement("text", "","",xmleLastElement,strText);
				} else if (arrElementsUnparsed[i].ToString().Substring(arrElementsUnparsed[i].Length-2,1) != "/") {
					xmleLastElement = xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement(strType, strTag,strAttributes,xmleLastElement,strText);
					iElements++;
					if (strText != "") xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement("text", "","",xmleLastElement,strText);
				} else {
					xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement(strType, strTag,strAttributes,xmleLastElement,strText);
					if (strText != "") xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement("text", "","",xmleLastElement,strText);
				}
			} else {
				iElements--;
				if (xmleLastElement != null) {
					xmleLastElement = xmleLastElement.parentElement;
					if (strText != "") {
						xmleLastElement.text += strText;
						xmleLastElement.childElements[Array.IndexOf(xmleLastElement.childElements, null)] = new XMLElement("text", "","",xmleLastElement,strText);
					}
				}
			}
		}
	}
	
	public class XMLElement {
		public string type;
		public string name;
		public string attributeString;
		public string[,] attributes;
		public XMLElement[] childElements;
		private XMLElement m_parentElement;
		public string text; // text of element
		
		public XMLElement(string strType, string strName, string strAttributes, XMLElement xmlParent, string strText) {
//if (xmlParent != null) Debug.Comment("!!!" + strName + " : " + xmlParent.name);
//else Debug.Comment("!!!" + strName);
			this.type = strType;
			this.name = strName;
			this.attributeString = strAttributes;
			this.attributes = null;
			this.childElements = new XMLElement[512];
			this.m_parentElement = xmlParent;
			this.text = strText; // text of element
	
		}	

		public XMLElement parentElement {
			get {
				return m_parentElement;
			}
		}
		
		public string getText() {
			if (this.type == "text" || this.type == "cdata") {
				return this.text;
			} else if (Array.IndexOf(this.childElements, null) != 0) {
				string L = "";
				for (int i=0; i<Array.IndexOf(this.childElements, null); i++) {
					L += this.childElements[i].getText();
				}
				return L;
			} else return "";
		}
		
		public XMLElement childElement(string strElementName) {
			for (int i=0; i<Array.IndexOf(this.childElements, null); i++) if (this.childElements[i].name == strElementName) return this.childElements[i];
			return null;
		}

		public XMLElement childElement(int iElementIndex) {
                if (iElementIndex < this.childElements.Length)
                {
                    try
                    {
                        return this.childElements[iElementIndex];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return null;
                    }
                }
            return null;
		}

		public string attribute(string strAttributeName) {
			if (this.attributes == null) {
				this.parseAttributes();
			}
			if (this.attributes.GetLength(0) != 0) for (int i=0; i<this.attributes.GetLength(0); i++) if (this.attributes[i, 0] == strAttributeName) return this.attributes[i, 1];
			return "";
		}		
		
		public void parseAttributes() {
			Regex reAttributes = new Regex(" ([^= ]*)="); // matches attributes
			Match[] arrTemp = new Match[512];
			string[,] arrAttributes = new string[512, 2];
			reAttributes.Matches(this.attributeString).CopyTo(arrTemp, 0);				
			if (Array.IndexOf(arrTemp, null) > 0) {
				for (int i=0; i<Array.IndexOf(arrTemp, null); i++) {
					arrAttributes[i, 0] = (new Regex("[= ]")).Replace(arrTemp[i].ToString(), "");
					arrAttributes[i, 1] = ParseAttribute(
						               this.attributeString, 
						               (new Regex("[= ]")).Replace(arrTemp[i].ToString(),"")
						               );
				}
				this.attributes = arrAttributes;
			} else {
				this.attributes = new string[0, 2];				
			}
		}
		
		private string ParseAttribute(string str, string Attribute) {
			str = str +  ">";
			Regex Attr;
			if (str.IndexOf(Attribute + "='") > -1) Attr = new Regex(".*" + Attribute + "='([^']*)'.*>");
			else Attr = new Regex(".*" + Attribute + "=\"([^\"]*)\".*>");
			return Attr.Replace(str, "$1");
		}
	}
}

}
