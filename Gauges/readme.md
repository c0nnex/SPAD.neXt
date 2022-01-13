#Saitek FIP Gauge Support in SPAD.neXt
##Overview
This readme is to provide a starter for information on Saitek FIP Gauges, in SPAD.neXt, mostly based on links to other sources plus some pointers.  A reliable link to Saitek FIP Gauge programming is not available from Saitek or Logitech (current manufacturer of the FIP).

General information on XML Gauges can be found in the FSX and Prepar3d SDKs.  Specifically the Prepar3d V5 SDK documentation on creating XML gauges can be found [here](http://www.prepar3d.com/SDK/SimObject%20Creation%20Kit/Panels%20and%20Gauges%20SDK/creating%20xml%20gauges.html#XML%20Gauge%20Reference).

Gauges XSD file in this repository is used to validate format of the gauge xml files for Saitek FIPs, with specific extentions for SPAD.neXt

Information on SPAD.neXt extensions can be found [here](https://www.spadnext.com/wiki/gauges:spad.next_extensions).

##Images
Images are stored under the main guage directory in a sub directory named **1024**.  
The Saitek FIP has a display resolution of 320 x 240
When adding images in the gauges XML, use the attribute ImageSizes="x,y,w,z" where x AND w are the image width, and y AND z are the image height (
