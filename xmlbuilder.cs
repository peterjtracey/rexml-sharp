using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JSXML {

public class XMLBuilder {
	public string XML;
	public XMLBuilder.XMLElement[] elements;

	public XMLBuilder() {
		this.XML = "";
		this.elements = new XMLElement[512];
	}
	
	public void load(string strXML) {
		this.load(new REXML(strXML).rootElement);
	}
	public void load(REXML.XMLElement xmleElem) {
		XMLIterator xmlBuilder = new XMLIterator(xmleElem);

		while (true) {
			if (xmlBuilder.xmleElem.type == "element") {
				this.addElementAt(xmlBuilder.xmleElem.name,xmlBuilder.xmleElem.attributes,xmlBuilder.xmleElem.attributeString, xmlBuilder.xmleElem.text, Array.IndexOf(this.elements, null), xmlBuilder.iElemLevel);
			}
			if (!xmlBuilder.GetNextNode()) break;
		}
		for (int i=0; i<Array.IndexOf(this.elements, null); i++) this.elements[i].index = i;
	}
	
	public XMLBuilder.XMLElement rootElement {
		get {
			return this.elements[0];
		}
	}
	
	public XMLBuilder.XMLElement element(int iIndex) {
		if (iIndex < Array.IndexOf(this.elements, null)) {
			return this.elements[iIndex];
		} else {
			return null;
		}
	}

	public int addElementAt(string strElement, string strAttributes, int iElemIndex, int iElemLevel) {
		return this.addElementAt(strElement, new string[0,0], strAttributes, "", iElemIndex, iElemLevel);
	}
	
	public int addElementAt(string strElement, string strAttributes, string strText, int iElemIndex, int iElemLevel) {
		return this.addElementAt(strElement, new string[0,0], strAttributes, strText, iElemIndex, iElemLevel);
	}
	
	public int addElementAt(string strElement, string[,] arrAttributes, string strAttributes, string strText, int iElemIndex, int iElemLevel) {
		if (iElemIndex < 0) iElemIndex = (Array.IndexOf(this.elements, null)>0) ? Array.IndexOf(this.elements, null)-1 : 0;
		if (iElemLevel < 0) iElemLevel = this.elements[iElemIndex-1].level;
		XMLElement Elem;
		int iAddIndex = iElemIndex;
		if (iElemIndex > 0) {
			for (int i=iElemIndex; i<Array.IndexOf(this.elements, null); i++) {
				Console.WriteLine(this.elements[i].level + " > " + (iElemLevel + 1));
				if (this.elements[i].level > iElemLevel + 1) iAddIndex++;
				else if (this.elements[i].level <= this.elements[iElemIndex].level) break;
			}
			Elem = new XMLBuilder.XMLElement(strElement,strAttributes,arrAttributes,strText,iElemLevel+1,this);
		} else {
			Elem = new XMLBuilder.XMLElement(strElement,strAttributes,arrAttributes,strText,1,this);
		}
		this.elements = XMLUtilities.Add(this.elements, iAddIndex, Elem);
		for (int i=iAddIndex; i<Array.IndexOf(this.elements, null); i++) this.elements[i].index = i;
		return iAddIndex;
	}

	public void insertElementAt(string strElement, string strAttributes, int iElemIndex, int iElemLevel) {
		this.insertElementAt(strElement, new string[0,0], strAttributes, "", iElemIndex, iElemLevel);
	}

	public void insertElementAt(string strElement, string strAttributes, string strText, int iElemIndex, int iElemLevel) {
		this.insertElementAt(strElement, new string[0,0], strAttributes, strText, iElemIndex, iElemLevel);
	}

	public void insertElementAt(string strElement, string[,] arrAttributes, string strAttributes, string strText, int iElemIndex, int iElemLevel) {
		if (iElemIndex < 0) iElemIndex = (Array.IndexOf(this.elements, null)>0) ? Array.IndexOf(this.elements, null)-1 : 0;
		if (iElemLevel < 0) iElemLevel = this.elements[iElemIndex-1].level;
		XMLElement Elem = null;
		int iAddIndex = iElemIndex;
		if (iElemIndex > 0 && iElemLevel > 0) {
			Elem = new XMLElement(strElement,strAttributes,arrAttributes,strText,iElemLevel+1,this);
		} else {
			Elem = new XMLElement(strElement,strAttributes,arrAttributes,strText,1,this);
		}
		this.elements = XMLUtilities.Add(this.elements, iAddIndex, Elem);
		for (int i=iAddIndex; i<Array.IndexOf(this.elements, null); i++) this.elements[i].index = i;
	}

	public void removeElement(int iElemIndex) { // todo: use iterator here to remove more than 1 level down
		while (this.elements[iElemIndex].childElement(0) != null) this.elements = XMLUtilities.Remove(this.elements, iElemIndex + 1);
		this.elements = XMLUtilities.Remove(this.elements, iElemIndex);
		for (int i=iElemIndex; i<Array.IndexOf(this.elements, null); i++) this.elements[i].index = i;
	}

	public int moveElement(int iElem1Index, int iElem2Index) {
		int i;
		int iNewIndex = -1;
		XMLBuilder.XMLElement[] arrElem1Elements = new XMLBuilder.XMLElement[512];
		arrElem1Elements[0] = this.elements[iElem1Index];
		XMLBuilder.XMLElement[] arrElem2Elements = new XMLBuilder.XMLElement[512];
		arrElem2Elements[0] = this.elements[iElem2Index];
		for (i=iElem1Index; i<Array.IndexOf(this.elements, null); i++) {
			if (this.elements[i].level > this.elements[iElem1Index].level) {
				arrElem1Elements[Array.IndexOf(arrElem1Elements, null)] = this.elements[i]; 
			} else if (i > iElem1Index) break;
		}
		for (i=iElem2Index; i<Array.IndexOf(this.elements, null); i++) {
			if (this.elements[i].level > this.elements[iElem2Index].level) {
				arrElem2Elements[Array.IndexOf(arrElem2Elements, null)] = this.elements[i]; 
			} else if (i>iElem2Index) break;
		}
		XMLBuilder.XMLElement[] arrMovedElements = new XMLBuilder.XMLElement[512];
		if (iElem1Index < iElem2Index) {
			// start to the 1st element
			for (i=0; i<iElem1Index; i++) { 
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			// end of 1st element to end of 2nd element
			for (i=iElem1Index+Array.IndexOf(arrElem1Elements, null); i<iElem2Index+Array.IndexOf(arrElem2Elements, null); i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			// 1st element and all child elements
			iNewIndex = Array.IndexOf(arrMovedElements, null);
			for (i=0; i<Array.IndexOf(arrElem1Elements, null); i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = arrElem1Elements[i]; 
			}
			// end of 2nd element to end
			for (i=iElem2Index+Array.IndexOf(arrElem2Elements, null); i<Array.IndexOf(this.elements, null); i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			this.elements = arrMovedElements;
		} else {
			// start to the 2nd element
			for (i=0; i<iElem2Index; i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			// 1st element and all child elements
			iNewIndex = Array.IndexOf(arrMovedElements, null);
			for (i=0; i<Array.IndexOf(arrElem1Elements, null); i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = arrElem1Elements[i]; 
			}
			// 2nd element to 1st element
			for (i=iElem2Index; i<iElem1Index; i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			// end of 1st element to end
			for (i=iElem1Index+Array.IndexOf(arrElem1Elements, null); i<Array.IndexOf(this.elements, null); i++) {
				arrMovedElements[Array.IndexOf(arrMovedElements, null)] = this.elements[i]; 
			}
			this.elements = arrMovedElements;
		}
		for (i=0; i<Array.IndexOf(this.elements, null); i++) this.elements[i].index = i;
		return iNewIndex;
	}
	
	public string generateXML(bool bXMLTag) {
		string strXML = "";
		string[] arrXML = new string[512];
		int lastelem;
		if (bXMLTag) strXML += "<?xml version=\"1.0\"?>\n\n";
		for (int i=0; i<Array.IndexOf(this.elements, null); i++) {
			strXML += XMLUtilities.RepeatChar("\t",this.elements[i].level-1);
			strXML += "<" + this.element(i).name; // open tag
			if (this.element(i).attributes != null && this.element(i).attributes.GetLength(0) != 0) {
				for (int j=0; j<this.element(i).attributes.GetLength(0) && this.element(i).attributes[j, 0] != null; j++) { // set attributes
					//???if (this.element(i).attributes[j]) {
						strXML += ' ' + this.element(i).attributes[j,0] + "=\"" + this.element(i).attributes[j,1] + '"';
					//}
				}
			}//??? else strXML += this.element(i).attributeString.replace(/[\/>]$/gi, "");
			if (((this.elements[i+1] != null && this.elements[i+1].level <= this.elements[i].level) || // next element is a lower or equal to
				(this.elements[i+1] == null && this.elements[i-1] != null)) // no next element, previous element
				&& this.element(i).text == "") {
				strXML += "/";
			}
			strXML += ">";
			if (this.element(i).text != "") strXML += this.element(i).text;
			else strXML += "\n";
			if (((this.elements[i+1] != null && this.elements[i+1].level <= this.elements[i].level) || // next element is a lower or equal to
				(this.elements[i+1] == null && this.elements[i-1] != null)) // no next element, previous element
				&& this.element(i).text != "") strXML += "</" + this.element(i).name + ">\n";
			if (this.elements[i+1] == null) {
				lastelem = i;
				for (int j=i; j>-1; j--) {
					if (this.elements[j].level >= this.elements[i].level) continue;
					else {
						if (this.elements[j].level < this.elements[lastelem].level) {
							strXML += XMLUtilities.RepeatChar("\t",this.elements[j].level-1) + "</" + this.element(j).name + ">\n";
							lastelem = j;
						}
					}
				}
			} else {
				if (this.elements[i+1].level < this.elements[i].level) {
					lastelem = i;
					for (int j=i; this.elements[j].level>=this.elements[i+1].level; j--) {
						if (this.elements[i] != null && this.elements[j] != null && this.elements[j].level < this.elements[i].level && this.elements[j].level < this.elements[lastelem].level) {
							strXML += XMLUtilities.RepeatChar("\t",this.elements[j].level-1) + "</" + this.element(j).name + ">\n";
							lastelem = j;
						}
					}
				}
			}
			if (strXML.Length > 1000) {
				arrXML[Array.IndexOf(arrXML, null)] = strXML;
				strXML = "";
			}
		}
		arrXML[Array.IndexOf(arrXML, null)] = strXML;
		return string.Join("", arrXML);
	}

	public class XMLElement : REXML.XMLElement {
		public int level;
		public int index;
		XMLBuilder xmlBuilder;
		
		public XMLElement(string strName, string strAttributes, string[,] arrAttributes, string strText, int iLevel, XMLBuilder xmlBuilder) :
			base("element", strName, strAttributes, null, strText) {
			if (strAttributes.Length != 0) {
				parseAttributes();
			} else {
				this.attributes = arrAttributes;
			}
			this.level = iLevel;
			this.index = -1;
			this.xmlBuilder = xmlBuilder;
		}
		
		public void setAttribute(string AttributeName, string Value) {
			if (this.attributes.GetLength(0) == 0) this.parseAttributes();
			if (this.attributes.GetLength(0) != 0) for (int i=0; i<this.attributes.GetLength(0); i++) if (this.attributes[i,0] == AttributeName) {
				this.attributes[i,1] = Value;
				return;
			}
			this.attributes[Array.IndexOf(this.attributes, null), 0] = AttributeName;
			this.attributes[Array.IndexOf(this.attributes, null), 1] = Value;
		}

		public void removeAttribute(string AttributeName, string Value) {
			if (Array.IndexOf(this.attributes, null) == 0) this.parseAttributes();
			if (Array.IndexOf(this.attributes, null) != 0) for (int i=0; i<Array.IndexOf(this.attributes, null); i++) if (this.attributes[i, 0] == AttributeName) {
				this.attributes = XMLUtilities.Remove(this.attributes, i);
				return;
			}
		}
		
		public new XMLBuilder.XMLElement parentElement {
			get {
				int i;
				for (i=this.index; this.xmlBuilder.element(i) != null && this.xmlBuilder.element(i).level != this.level-1; i--);
				return this.xmlBuilder.element(i);
			}
		}
		
		public new XMLElement childElement(string Child) {
			int iFind = -1;
			for (int i=this.index+1; i<Array.IndexOf(this.xmlBuilder.elements, null); i++) {
				if (this.xmlBuilder.elements[i].level == this.level+1) {
					iFind++;
					if (this.xmlBuilder.elements[i].name == Child) return this.xmlBuilder.elements[i];
				} else if (this.xmlBuilder.elements[i].level <= this.level) break;
			}
			return null;
		}

		public new XMLElement childElement(int Child) {
			int iFind = -1;
			for (int i=this.index+1; i<Array.IndexOf(this.xmlBuilder.elements, null); i++) {
				if (this.xmlBuilder.elements[i].level == this.level+1) {
					iFind++;
					if (iFind == Child) return this.xmlBuilder.elements[i];
				} else if (this.xmlBuilder.elements[i].level <= this.level) break;
			}
			return null;
		}
	}
	
	
}

}
