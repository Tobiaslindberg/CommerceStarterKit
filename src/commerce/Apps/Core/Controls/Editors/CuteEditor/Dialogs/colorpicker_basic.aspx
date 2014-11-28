<%@ Page Language="C#" Inherits="CuteEditor.EditorUtilityPage" %>
<script runat="server">
string GetDialogQueryString;
override protected void OnInit(EventArgs args)
{
	if(Context.Request.QueryString["Dialog"]=="Standard")
	{	
	if(Context.Request.QueryString["IsFrame"]==null)
	{
		string FrameSrc="colorpicker_basic.aspx?IsFrame=1&"+Request.ServerVariables["QUERY_STRING"];
		CuteEditor.CEU.WriteDialogOuterFrame(Context,"[[MoreColors]]",FrameSrc);
		Context.Response.End();
	}
	}
	string s="";
	if(Context.Request.QueryString["Dialog"]=="Standard")	
		s="&Dialog=Standard";
	
	GetDialogQueryString="Theme="+Context.Request.QueryString["Theme"]+s;	
	base.OnInit(args);
}
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<meta http-equiv="Page-Enter" content="blendTrans(Duration=0.1)" />
		<meta http-equiv="Page-Exit" content="blendTrans(Duration=0.1)" />
		<script type="text/javascript" src="Load.ashx?type=dialogscript&file=DialogHead.js"></script>
		<script type="text/javascript" src="Load.ashx?type=dialogscript&file=Dialog_ColorPicker.js"></script>
		<link href='Load.ashx?type=themecss&file=dialog.css&theme=[[_Theme_]]' type="text/css"
			rel="stylesheet" />
		<style type="text/css">
			.colorcell
			{
				width:16px;
				height:17px;
				cursor:hand;
			}
			.colordiv,.customdiv
			{
				border:solid 1px #808080;
				width:16px;
				height:17px;
				font-size:1px;
			}
		</style>
		<title>[[NamedColors]]</title>
		<script>
								
		var OxOfc92=["Green","#008000","Lime","#00FF00","Teal","#008080","Aqua","#00FFFF","Navy","#000080","Blue","#0000FF","Purple","#800080","Fuchsia","#FF00FF","Maroon","#800000","Red ","#FF0000","Olive","#808000","Yellow","#FFFF00","White","#FFFFFF","Silver","#C0C0C0","Gray","#808080","Black","#000000","DarkOliveGreen","#556B2F","DarkGreen","#006400","DarkSlateGray","#2F4F4F","SlateGray","#708090","DarkBlue","#00008B","MidnightBlue","#191970","Indigo","#4B0082","DarkMagenta","#8B008B","Brown","#A52A2A","DarkRed","#8B0000","Sienna","#A0522D","SaddleBrown","#8B4513","DarkGoldenrod","#B8860B","Beige","#F5F5DC","HoneyDew","#F0FFF0","DimGray","#696969","OliveDrab","#6B8E23","ForestGreen","#228B22","DarkCyan","#008B8B","LightSlateGray","#778899","MediumBlue","#0000CD","DarkSlateBlue","#483D8B","DarkViolet","#9400D3","MediumVioletRed","#C71585","IndianRed","#CD5C5C","Firebrick","#B22222","Chocolate","#D2691E","Peru","#CD853F","Goldenrod","#DAA520","LightGoldenrodYellow","#FAFAD2","MintCream","#F5FFFA","DarkGray","#A9A9A9","YellowGreen","#9ACD32","SeaGreen","#2E8B57","CadetBlue","#5F9EA0","SteelBlue","#4682B4","RoyalBlue","#4169E1","BlueViolet","#8A2BE2","DarkOrchid","#9932CC","DeepPink","#FF1493","RosyBrown","#BC8F8F","Crimson","#DC143C","DarkOrange","#FF8C00","BurlyWood","#DEB887","DarkKhaki","#BDB76B","LightYellow","#FFFFE0","Azure","#F0FFFF","LightGray","#D3D3D3","LawnGreen","#7CFC00","MediumSeaGreen","#3CB371","LightSeaGreen","#20B2AA","DeepSkyBlue","#00BFFF","DodgerBlue","#1E90FF","SlateBlue","#6A5ACD","MediumOrchid","#BA55D3","PaleVioletRed","#DB7093","Salmon","#FA8072","OrangeRed","#FF4500","SandyBrown","#F4A460","Tan","#D2B48C","Gold","#FFD700","Ivory","#FFFFF0","GhostWhite","#F8F8FF","Gainsboro","#DCDCDC","Chartreuse","#7FFF00","LimeGreen","#32CD32","MediumAquamarine","#66CDAA","DarkTurquoise","#00CED1","CornflowerBlue","#6495ED","MediumSlateBlue","#7B68EE","Orchid","#DA70D6","HotPink","#FF69B4","LightCoral","#F08080","Tomato","#FF6347","Orange","#FFA500","Bisque","#FFE4C4","Khaki","#F0E68C","Cornsilk","#FFF8DC","Linen","#FAF0E6","WhiteSmoke","#F5F5F5","GreenYellow","#ADFF2F","DarkSeaGreen","#8FBC8B","Turquoise","#40E0D0","MediumTurquoise","#48D1CC","SkyBlue","#87CEEB","MediumPurple","#9370DB","Violet","#EE82EE","LightPink","#FFB6C1","DarkSalmon","#E9967A","Coral","#FF7F50","NavajoWhite","#FFDEAD","BlanchedAlmond","#FFEBCD","PaleGoldenrod","#EEE8AA","Oldlace","#FDF5E6","Seashell","#FFF5EE","PaleGreen","#98FB98","SpringGreen","#00FF7F","Aquamarine","#7FFFD4","PowderBlue","#B0E0E6","LightSkyBlue","#87CEFA","LightSteelBlue","#B0C4DE","Plum","#DDA0DD","Pink","#FFC0CB","LightSalmon","#FFA07A","Wheat","#F5DEB3","Moccasin","#FFE4B5","AntiqueWhite","#FAEBD7","LemonChiffon","#FFFACD","FloralWhite","#FFFAF0","Snow","#FFFAFA","AliceBlue","#F0F8FF","LightGreen","#90EE90","MediumSpringGreen","#00FA9A","PaleTurquoise","#AFEEEE","LightCyan","#E0FFFF","LightBlue","#ADD8E6","Lavender","#E6E6FA","Thistle","#D8BFD8","MistyRose","#FFE4E1","Peachpuff","#FFDAB9","PapayaWhip","#FFEFD5"];var colorlist=[{n:OxOfc92[0x0],h:OxOfc92[0x1]},{n:OxOfc92[0x2],h:OxOfc92[0x3]},{n:OxOfc92[0x4],h:OxOfc92[0x5]},{n:OxOfc92[0x6],h:OxOfc92[0x7]},{n:OxOfc92[0x8],h:OxOfc92[0x9]},{n:OxOfc92[0xa],h:OxOfc92[0xb]},{n:OxOfc92[0xc],h:OxOfc92[0xd]},{n:OxOfc92[0xe],h:OxOfc92[0xf]},{n:OxOfc92[0x10],h:OxOfc92[0x11]},{n:OxOfc92[0x12],h:OxOfc92[0x13]},{n:OxOfc92[0x14],h:OxOfc92[0x15]},{n:OxOfc92[0x16],h:OxOfc92[0x17]},{n:OxOfc92[0x18],h:OxOfc92[0x19]},{n:OxOfc92[0x1a],h:OxOfc92[0x1b]},{n:OxOfc92[0x1c],h:OxOfc92[0x1d]},{n:OxOfc92[0x1e],h:OxOfc92[0x1f]}];var colormore=[{n:OxOfc92[0x20],h:OxOfc92[0x21]},{n:OxOfc92[0x22],h:OxOfc92[0x23]},{n:OxOfc92[0x24],h:OxOfc92[0x25]},{n:OxOfc92[0x26],h:OxOfc92[0x27]},{n:OxOfc92[0x28],h:OxOfc92[0x29]},{n:OxOfc92[0x2a],h:OxOfc92[0x2b]},{n:OxOfc92[0x2c],h:OxOfc92[0x2d]},{n:OxOfc92[0x2e],h:OxOfc92[0x2f]},{n:OxOfc92[0x30],h:OxOfc92[0x31]},{n:OxOfc92[0x32],h:OxOfc92[0x33]},{n:OxOfc92[0x34],h:OxOfc92[0x35]},{n:OxOfc92[0x36],h:OxOfc92[0x37]},{n:OxOfc92[0x38],h:OxOfc92[0x39]},{n:OxOfc92[0x3a],h:OxOfc92[0x3b]},{n:OxOfc92[0x3c],h:OxOfc92[0x3d]},{n:OxOfc92[0x3e],h:OxOfc92[0x3f]},{n:OxOfc92[0x40],h:OxOfc92[0x41]},{n:OxOfc92[0x42],h:OxOfc92[0x43]},{n:OxOfc92[0x44],h:OxOfc92[0x45]},{n:OxOfc92[0x46],h:OxOfc92[0x47]},{n:OxOfc92[0x48],h:OxOfc92[0x49]},{n:OxOfc92[0x4a],h:OxOfc92[0x4b]},{n:OxOfc92[0x4c],h:OxOfc92[0x4d]},{n:OxOfc92[0x4e],h:OxOfc92[0x4f]},{n:OxOfc92[0x50],h:OxOfc92[0x51]},{n:OxOfc92[0x52],h:OxOfc92[0x53]},{n:OxOfc92[0x54],h:OxOfc92[0x55]},{n:OxOfc92[0x56],h:OxOfc92[0x57]},{n:OxOfc92[0x58],h:OxOfc92[0x59]},{n:OxOfc92[0x5a],h:OxOfc92[0x5b]},{n:OxOfc92[0x5c],h:OxOfc92[0x5d]},{n:OxOfc92[0x5e],h:OxOfc92[0x5f]},{n:OxOfc92[0x60],h:OxOfc92[0x61]},{n:OxOfc92[0x62],h:OxOfc92[0x63]},{n:OxOfc92[0x64],h:OxOfc92[0x65]},{n:OxOfc92[0x66],h:OxOfc92[0x67]},{n:OxOfc92[0x68],h:OxOfc92[0x69]},{n:OxOfc92[0x6a],h:OxOfc92[0x6b]},{n:OxOfc92[0x6c],h:OxOfc92[0x6d]},{n:OxOfc92[0x6e],h:OxOfc92[0x6f]},{n:OxOfc92[0x70],h:OxOfc92[0x71]},{n:OxOfc92[0x72],h:OxOfc92[0x73]},{n:OxOfc92[0x74],h:OxOfc92[0x75]},{n:OxOfc92[0x76],h:OxOfc92[0x77]},{n:OxOfc92[0x78],h:OxOfc92[0x79]},{n:OxOfc92[0x7a],h:OxOfc92[0x7b]},{n:OxOfc92[0x7c],h:OxOfc92[0x7d]},{n:OxOfc92[0x7e],h:OxOfc92[0x7f]},{n:OxOfc92[0x80],h:OxOfc92[0x81]},{n:OxOfc92[0x82],h:OxOfc92[0x83]},{n:OxOfc92[0x84],h:OxOfc92[0x85]},{n:OxOfc92[0x86],h:OxOfc92[0x87]},{n:OxOfc92[0x88],h:OxOfc92[0x89]},{n:OxOfc92[0x8a],h:OxOfc92[0x8b]},{n:OxOfc92[0x8c],h:OxOfc92[0x8d]},{n:OxOfc92[0x8e],h:OxOfc92[0x8f]},{n:OxOfc92[0x90],h:OxOfc92[0x91]},{n:OxOfc92[0x92],h:OxOfc92[0x93]},{n:OxOfc92[0x94],h:OxOfc92[0x95]},{n:OxOfc92[0x96],h:OxOfc92[0x97]},{n:OxOfc92[0x98],h:OxOfc92[0x99]},{n:OxOfc92[0x9a],h:OxOfc92[0x9b]},{n:OxOfc92[0x9c],h:OxOfc92[0x9d]},{n:OxOfc92[0x9e],h:OxOfc92[0x9f]},{n:OxOfc92[0xa0],h:OxOfc92[0xa1]},{n:OxOfc92[0xa2],h:OxOfc92[0xa3]},{n:OxOfc92[0xa4],h:OxOfc92[0xa5]},{n:OxOfc92[0xa6],h:OxOfc92[0xa7]},{n:OxOfc92[0xa8],h:OxOfc92[0xa9]},{n:OxOfc92[0xaa],h:OxOfc92[0xab]},{n:OxOfc92[0xac],h:OxOfc92[0xad]},{n:OxOfc92[0xae],h:OxOfc92[0xaf]},{n:OxOfc92[0xb0],h:OxOfc92[0xb1]},{n:OxOfc92[0xb2],h:OxOfc92[0xb3]},{n:OxOfc92[0xb4],h:OxOfc92[0xb5]},{n:OxOfc92[0xb6],h:OxOfc92[0xb7]},{n:OxOfc92[0xb8],h:OxOfc92[0xb9]},{n:OxOfc92[0xba],h:OxOfc92[0xbb]},{n:OxOfc92[0xbc],h:OxOfc92[0xbd]},{n:OxOfc92[0xbe],h:OxOfc92[0xbf]},{n:OxOfc92[0xc0],h:OxOfc92[0xc1]},{n:OxOfc92[0xc2],h:OxOfc92[0xc3]},{n:OxOfc92[0xc4],h:OxOfc92[0xc5]},{n:OxOfc92[0xc6],h:OxOfc92[0xc7]},{n:OxOfc92[0xc8],h:OxOfc92[0xc9]},{n:OxOfc92[0xca],h:OxOfc92[0xcb]},{n:OxOfc92[0xcc],h:OxOfc92[0xcd]},{n:OxOfc92[0xce],h:OxOfc92[0xcf]},{n:OxOfc92[0xd0],h:OxOfc92[0xd1]},{n:OxOfc92[0xd2],h:OxOfc92[0xd3]},{n:OxOfc92[0xd4],h:OxOfc92[0xd5]},{n:OxOfc92[0xd6],h:OxOfc92[0xd7]},{n:OxOfc92[0xd8],h:OxOfc92[0xd9]},{n:OxOfc92[0xda],h:OxOfc92[0xdb]},{n:OxOfc92[0xdc],h:OxOfc92[0xdd]},{n:OxOfc92[0x9c],h:OxOfc92[0x9d]},{n:OxOfc92[0xde],h:OxOfc92[0xdf]},{n:OxOfc92[0xe0],h:OxOfc92[0xe1]},{n:OxOfc92[0xe2],h:OxOfc92[0xe3]},{n:OxOfc92[0xe4],h:OxOfc92[0xe5]},{n:OxOfc92[0xe6],h:OxOfc92[0xe7]},{n:OxOfc92[0xe8],h:OxOfc92[0xe9]},{n:OxOfc92[0xea],h:OxOfc92[0xeb]},{n:OxOfc92[0xec],h:OxOfc92[0xed]},{n:OxOfc92[0xee],h:OxOfc92[0xef]},{n:OxOfc92[0xf0],h:OxOfc92[0xf1]},{n:OxOfc92[0xf2],h:OxOfc92[0xf3]},{n:OxOfc92[0xf4],h:OxOfc92[0xf5]},{n:OxOfc92[0xf6],h:OxOfc92[0xf7]},{n:OxOfc92[0xf8],h:OxOfc92[0xf9]},{n:OxOfc92[0xfa],h:OxOfc92[0xfb]},{n:OxOfc92[0xfc],h:OxOfc92[0xfd]},{n:OxOfc92[0xfe],h:OxOfc92[0xff]},{n:OxOfc92[0x100],h:OxOfc92[0x101]},{n:OxOfc92[0x102],h:OxOfc92[0x103]},{n:OxOfc92[0x104],h:OxOfc92[0x105]},{n:OxOfc92[0x106],h:OxOfc92[0x107]},{n:OxOfc92[0x108],h:OxOfc92[0x109]},{n:OxOfc92[0x10a],h:OxOfc92[0x10b]},{n:OxOfc92[0x10c],h:OxOfc92[0x10d]},{n:OxOfc92[0x10e],h:OxOfc92[0x10f]},{n:OxOfc92[0x110],h:OxOfc92[0x111]}];
		
		</script>
	</head>
	<body>
		<div id="container">
			<div class="tab-pane-control tab-pane" id="tabPane1">
				<div class="tab-row">
					<h2 class="tab">
						<a tabindex="-1" href='colorpicker.aspx?<%=GetDialogQueryString%>'>
							<span style="white-space:nowrap;">
								[[WebPalette]]
							</span>
						</a>
					</h2>
					<h2 class="tab selected">
							<a tabindex="-1" href='colorpicker_basic.aspx?<%=GetDialogQueryString%>'>
								<span style="white-space:nowrap;">
									[[NamedColors]]
								</span>
							</a>
					</h2>
					<h2 class="tab">
							<a tabindex="-1" href='colorpicker_more.aspx?<%=GetDialogQueryString%>'>
								<span style="white-space:nowrap;">
									[[CustomColor]]
								</span>
							</a>
					</h2>
				</div>
				<div class="tab-page">			
					<table class="colortable" align="center">
						<tr>
							<td colspan="16" height="16"><p align="left">Basic:
								</p>
							</td>
						</tr>
						<tr>
							<script>
								var OxO4b53=["length","\x3Ctd class=\x27colorcell\x27\x3E\x3Cdiv class=\x27colordiv\x27 style=\x27background-color:","\x27 title=\x27"," ","\x27 cname=\x27","\x27 cvalue=\x27","\x27\x3E\x3C/div\x3E\x3C/td\x3E",""];var arr=[];for(var i=0x0;i<colorlist[OxO4b53[0x0]];i++){ arr.push(OxO4b53[0x1]) ; arr.push(colorlist[i].n) ; arr.push(OxO4b53[0x2]) ; arr.push(colorlist[i].n) ; arr.push(OxO4b53[0x3]) ; arr.push(colorlist[i].h) ; arr.push(OxO4b53[0x4]) ; arr.push(colorlist[i].n) ; arr.push(OxO4b53[0x5]) ; arr.push(colorlist[i].h) ; arr.push(OxO4b53[0x6]) ;} ; document.write(arr.join(OxO4b53[0x7])) ;
							</script>
						</tr>
						<tr>
							<td colspan="16" height="12"><p align="left"></p>
							</td>
						</tr>
						<tr>
							<td colspan="16"><p align="left">Additional:
								</p>
							</td>
						</tr>
						<script>
							var OxO4ea3=["length","\x3Ctr\x3E","\x3Ctd class=\x27colorcell\x27\x3E\x3Cdiv class=\x27colordiv\x27 style=\x27background-color:","\x27 title=\x27"," ","\x27 cname=\x27","\x27 cvalue=\x27","\x27\x3E\x3C/div\x3E\x3C/td\x3E","\x3C/tr\x3E",""];var arr=[];for(var i=0x0;i<colormore[OxO4ea3[0x0]];i++){if(i%0x10==0x0){ arr.push(OxO4ea3[0x1]) ;} ; arr.push(OxO4ea3[0x2]) ; arr.push(colormore[i].n) ; arr.push(OxO4ea3[0x3]) ; arr.push(colormore[i].n) ; arr.push(OxO4ea3[0x4]) ; arr.push(colormore[i].h) ; arr.push(OxO4ea3[0x5]) ; arr.push(colormore[i].n) ; arr.push(OxO4ea3[0x6]) ; arr.push(colormore[i].h) ; arr.push(OxO4ea3[0x7]) ;if(i%0x10==0xf){ arr.push(OxO4ea3[0x8]) ;} ;} ;if(colormore%0x10>0x0){ arr.push(OxO4ea3[0x8]) ;} ; document.write(arr.join(OxO4ea3[0x9])) ;
						</script>
						<tr>
							<td colspan="16" height="8">
							</td>
						</tr>
						<tr>
							<td colspan="16" height="12">
								<input checked id="CheckboxColorNames" style="width: 16px; height: 20px" type="checkbox">
								<span style="width: 118px;">Use color names</span>
							</td>
						</tr>
						<tr>
							<td colspan="16" height="12">
							</td>
						</tr>
						<tr>
							<td colspan="16" valign="middle" height="24">
							<span style="height:24px;width:50px;vertical-align:middle;">Color : </span>&nbsp;
							<input type="text" id="divpreview" size="7" maxlength="7" style="width:180px;height:24px;border:#a0a0a0 1px solid; Padding:4;"/>
					
							</td>
						</tr>
				</table>
			</div>
		</div>
		<div id="container-bottom">
			<input type="button" id="buttonok" value="[[OK]]" class="formbutton" style="width:70px"	onclick="do_insert();" /> 
			&nbsp;&nbsp;&nbsp;&nbsp; 
			<input type="button" id="buttoncancel" value="[[Cancel]]" class="formbutton" style="width:70px"	onclick="do_Close();" />	
		</div>
	</div>
	</body>
</html>

