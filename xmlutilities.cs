using System;

namespace JSXML {

public class XMLUtilities {
	public static XMLBuilder.XMLElement[] Add(XMLBuilder.XMLElement[] arrElements, int iIndex, XMLBuilder.XMLElement xmleElement) {
		XMLBuilder.XMLElement[] tmparr = new XMLBuilder.XMLElement[512];
		for (int i=0; i<Array.IndexOf(arrElements, null); i++) {
			if (i==iIndex) tmparr[Array.IndexOf(tmparr, null)] = xmleElement;
			tmparr[Array.IndexOf(tmparr, null)] = arrElements[i];
		}
		if (tmparr[iIndex] == null) tmparr[iIndex] = xmleElement;
		return tmparr;
	}		

	public static XMLBuilder.XMLElement[] Remove(XMLBuilder.XMLElement[] arrElements, int iIndex) {
		XMLBuilder.XMLElement[] tmparr = new XMLBuilder.XMLElement[512];
		for (int i=0; i<Array.IndexOf(arrElements, null); i++) if (i!=iIndex) tmparr[Array.IndexOf(tmparr, null)] = arrElements[i];
		return tmparr;
	}

	public static string[,] Remove(string[,] arrAttributes, int iIndex) {
		string[,] tmparr = new string[512, 2];
		for (int i=0; i<Array.IndexOf(arrAttributes, null); i++) {
			if (i!=iIndex) {
				tmparr[Array.IndexOf(tmparr, null), 0] = arrAttributes[i, 0];
				tmparr[Array.IndexOf(tmparr, null), 1] = arrAttributes[i, 1];
			}
		}
		return tmparr;
	}

	public static string RepeatChar(string sChar, int iNum) {
		string L = "";
		for (int i=0; i<iNum; i++) L += sChar;
		return L;
	}
	
}

}
