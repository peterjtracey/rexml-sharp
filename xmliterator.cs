using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JSXML {

public class XMLIterator {
	public REXML.XMLElement xmleElem;
	public int iElemIndex;
	int[] arrElemIndex;
	public int iElemLevel;
	
	public XMLIterator(REXML.XMLElement xmleElem) {
		this.xmleElem = xmleElem;
		
		this.iElemIndex = 0;
		this.arrElemIndex = new int[512];
		this.arrElemIndex[0] = 0;
		this.iElemLevel = 0;
		this.arrElemIndex[this.iElemLevel] = -1;
	}
	
	public bool GetNextNode() {
		if (this.xmleElem == null || this.iElemLevel<0) return false;
		if (Array.IndexOf(this.xmleElem.childElements, null) > 0) {  // move up
			this.arrElemIndex[this.iElemLevel]++;
			this.iElemIndex++;
			this.iElemLevel++;
			this.arrElemIndex[this.iElemLevel] = 0;
			this.xmleElem = this.xmleElem.childElements[0];
		} else { // move next
			this.iElemIndex++;
			this.arrElemIndex[this.iElemLevel]++;
			if (this.xmleElem.parentElement != null &&
			    Array.IndexOf(this.xmleElem.parentElement.childElements, null) != 0 &&
			    this.arrElemIndex[this.iElemLevel] < Array.IndexOf(this.xmleElem.parentElement.childElements, null)) {
			    	this.xmleElem = this.xmleElem.parentElement.childElements[this.arrElemIndex[this.iElemLevel]];
		    } else {
				if (this.iElemLevel>0) { // move down
					for (; this.iElemLevel > 0; this.iElemLevel--) {
						if (this.xmleElem.parentElement != null && this.xmleElem.parentElement.childElements[this.arrElemIndex[this.iElemLevel]] != null) {
							this.xmleElem = this.xmleElem.parentElement.childElements[this.arrElemIndex[this.iElemLevel]];
							this.iElemLevel++;
							Array.Copy(this.arrElemIndex, this.arrElemIndex, this.iElemLevel+1);
							break;
						} else {
							this.xmleElem = this.xmleElem.parentElement;
						}
					}
					this.iElemLevel--;
				} else {
					return false;
				}
			}
		}
		return (this.xmleElem != null && this.iElemLevel > -1);
	}
}

}
