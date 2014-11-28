
function CuteEditorDatePickerInitialize(element)
{

if(!element.runtimeStyle)element.runtimeStyle=element.style;

var inithtml="<table id='csdpid_tablepanel' border='0' style='height:100%;font:normal 11px Tahoma;border:solid 1px #808080;background-color:#f2f2f2;cursor:default;'>	\
	<tr height='21'>	\
		<td>	\
			<table border='0'>	\
				<tr>	\
					<td id='csdpid_mprev' style='font-size:8pt;font-family:webdings;'>7</td>	\
					<td><select id='csdpid_msel' style='font:normal 11px Tahoma;'>	\
							<option>January</option>	\
							<option>February</option>	\
							<option>March</option>	\
							<option>April</option>	\
							<option>May</option>	\
							<option>June</option>	\
							<option>July</option>	\
							<option>August</option>	\
							<option>September</option>	\
							<option>October</option>	\
							<option>November</option>	\
							<option>December</option>	\
						</select></td>	\
					<td id='csdpid_mnext' style='font-size:8pt;font-family:webdings;'>8</td>	\
					<td id='csdpid_dprev' style='font-size:8pt;font-family:webdings;'>7</td>	\
					<td><select id='csdpid_dsel' style='font:normal 11px Tahoma;'><option>11</option>	\
						</select></td>	\
					<td id='csdpid_dnext' style='font-size:8pt;font-family:webdings;'>8</td>	\
				</tr>	\
			</table>	\
		</td>	\
		<td align='right'>	\
			<table border='0'>	\
				<tr>	\
					<td id='csdpid_yprev' style='font-size:8pt;font-family:webdings;' title=''>7</td>	\
					<td><select id='csdpid_ysel' style='font:normal 11px Tahoma;'><option>2000</option>	\
						</select></td>	\
					<td id='csdpid_ynext' style='font-size:8pt;font-family:webdings;' title=''>8</td>	\
				</tr>	\
			</table>	\
		</td>	\
	</tr>	\
	<tr height='2'>	\
		<td align='center' colspan='2'>	\
		</td>	\
	</tr>	\
	<tr>	\
		<td colspan='2'>	\
			<table id='csdpid_tabledays' height='162' border='1' bordercolor='#cccccc'	\
				style='border-collapse:collapse;'>	\
				<colgroup>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
					<col align='center' style='font:normal 11px Tahoma;padding-top:3px;width:35px;'></col>	\
				</colgroup>	\
				<tr>	\
					<td id='csdpid_weeksun' weeknum='0' title=''>Sun</td>	\
					<td id='csdpid_weekmon' weeknum='1' title=''>Mon</td>	\
					<td id='csdpid_weektue' weeknum='2' title=''>Tue</td>	\
					<td id='csdpid_weekwed' weeknum='3' title=''>Wed</td>	\
					<td id='csdpid_weekthu' weeknum='4' title=''>Thu</td>	\
					<td id='csdpid_weekfri' weeknum='5' title=''>Fri</td>	\
					<td id='csdpid_weeksat' weeknum='6' title=''>Sat</td>	\
				</tr>	\
			</table>	\
		</td>	\
	</tr>	\
	<tr>	\
		<td>	\
			<button id='csdpid_btntd' hidefocus='1' onfocus='blur()' style='width:60px;cursor:hand;border:1px outset;font:normal 11px Tahoma;padding-top:2px;'	\
				title=''>Today</button>	\
			<button id='csdpid_btnbl' hidefocus='1' onfocus='blur()' style='width:60px;cursor:hand;border:1px outset;font:normal 11px Tahoma;padding-top:2px;'	\
				title=''>Blank</button>	\
		</td>	\
		<td>	\
			<button id='csdpid_btnok' hidefocus='1' onfocus='blur()' style='width:60px;cursor:hand;border:1px outset;font:normal 11px Tahoma;padding-top:2px;'	\
				title=''>OK</button>	\
		</td>	\
	</tr>	\
</table>	\
	\
"

function _contains(p,e)
{
	for(;e;e=e.parentNode)
		if(e==p)
			return true;
}

function AttachEvent(e,name,func)
{
	if(name.substring(0,2)=="on")name=name.substring(2);
	if(e.attachEvent)
		e.attachEvent("on"+name,func);
	else
		e.addEventListener(name,func,1);
}
function DetachEvent(e,name,func)
{
	if(name.substring(0,2)=="on")name=name.substring(2);
	if(e.detachEvent)
		e.detachEvent("on"+name,func);
	else
		e.removeEventListener(name,func,1);
}


//Old_Position_Functions.js

/****************************************************************\
	Position Functions
\****************************************************************/
//get the position of a element ( by the scroll offset )

function GetScrollPostion(e)
{
 var b=window.document.body;
 var p=b;
 if(window.document.compatMode=="CSS1Compat")
 {
  p=window.document.documentElement;
 }
 
 var l=0;
 var t=0;
 for(var e1=e;e1!=null&&e1!=b;e1=e1.offsetParent)
 {
  l+=e1.offsetLeft-e1.scrollLeft;
  t+=e1.offsetTop-e1.scrollTop;
 }
 return {left:l,top:t}
}

//get the position of a element ( by the client offset )
function GetClientPosition(e)
{
	var b=window.document.body;
	var p=b;
	if(window.document.compatMode=="CSS1Compat")
	{
		p=window.document.documentElement;
	}
	
	if(e==b)return {left:-p.scrollLeft,top:-p.scrollTop};
	
	if(e.getBoundingClientRect)
	{
		var b=e.getBoundingClientRect();
		return {left:b.left-p.clientLeft,top:b.top-p.clientTop};
	}
	
	var l=0;
	var t=0;
	for(var e1=e;e1!=null&&e1!=b;e1=e1.offsetParent)
	{
		l+=e1.offsetLeft;
		t+=e1.offsetTop;
	}
	return {left:l-p.scrollLeft,top:t-p.scrollTop}
}
//get absolute or relative parent
function GetStandParent(e)
{
	var view;
	if(!e.currentStyle)view=e.ownerDocument.defaultView;
	for(var pe=e.parentNode;pe!=null&&pe.nodeType==1;pe=pe.parentNode)
	{
		var sp;
		if(pe.currentStyle)sp=pe.currentStyle.position;
		else view.getComputedStyle(pe, "").getPropertyValue("position")
		if(sp=="absolute"||sp=="relative")
			return pe;
	}
	return (e.ownerDocument||e.document).body;
}
//calc the position of floate that relative to e
function CalcPosition(floate,e)
{
	//var b=window.document.body;
	var epos=GetScrollPostion(e);
	var spos=GetScrollPostion(GetStandParent(floate));
	var s=GetStandParent(floate);
	return {left:epos.left-spos.left-(s.clientLeft||0),top:epos.top-spos.top-(s.clientTop||0)};
}

//get the best position to put the floate
function AdjustMirror(floate,e,pos)
{
	//c:Client,f:floate,e:e,p:floate"s StandParent,m:Mirror

	//get the size of window
	var cw=window.document.body.clientWidth;
	var ch=window.document.body.clientHeight;
	if(window.document.compatMode=="CSS1Compat")
	{
		cw=window.document.documentElement.clientWidth;
		ch=window.document.documentElement.clientHeight;
	}
	
	//get the size of float element
 	var fw=floate.offsetWidth;
	var fh=floate.offsetHeight;
	
	var pcpos=GetClientPosition(GetStandParent(floate));
	
	//get the center of float element
	var fmpos={left:pcpos.left+pos.left+fw/2,top:pcpos.top+pos.top+fh/2};//

	var empos={left:pcpos.left+pos.left,top:pcpos.top+pos.top};

	if(e!=null)
	{
		var ecpos=GetClientPosition(e);
		
		//get the center of the relative element
		empos={left:ecpos.left+e.offsetWidth/2,top:ecpos.top+e.offsetHeight/2};//
	}
	
	var allowmove=true;
 
	//left<-->right

	if(fmpos.left-fw/2<0)
	{
		if((empos.left*2-fmpos.left)+fw/2<=cw)
		{
			fmpos.left=empos.left*2-fmpos.left;
		}
		else if(allowmove)
		{
			fmpos.left=fw/2+4;
		}
	}
	else if(fmpos.left+fw/2>cw)
	{
		if((empos.left*2-fmpos.left)-fw/2>=0)
		{
			fmpos.left=empos.left*2-fmpos.left;
		}
		else if(allowmove)
		{
			fmpos.left=cw-fw/2-4;
		}
	}
	

	//top<-->bottom

	if(fmpos.top-fh/2<0)
	{
		if((empos.top*2-fmpos.top)+fh/2<=ch)
		{
			fmpos.top=empos.top*2-fmpos.top;
		}
		else if(allowmove)
		{
			fmpos.top=fh/2+4;
		}
	}
	else if(fmpos.top+fh/2>ch)
	{
		if((empos.top*2-fmpos.top)-fh/2>=0)
		{
			fmpos.top=empos.top*2-fmpos.top;
		}
		else if(allowmove)
		{
			fmpos.top=ch-fh/2-4;
		}
	}
 
	pos.left=fmpos.left-pcpos.left-fw/2;
	pos.top=fmpos.top-pcpos.top-fh/2;
}



function AddDays(date,value)
{
	date.setDate(date.getDate()+value);
}
function AddMonths(date,value)
{
	date.setMonth(date.getMonth()+value);
}
function AddYears(date,value)
{
	date.setFullYear(date.getFullYear()+value);
}
function IsToday(date)
{
	return IsDateEquals(date,new Date());
}
function IsThisMonth(date)
{
	return IsMonthEquals(date,new Date());
}
function IsMonthEquals(date1,date2)
{
	return date1.getMonth()==date2.getMonth()&&date1.getFullYear()==date2.getFullYear();
}
function IsDateEquals(date1,date2)
{
	return date1.getDate()==date2.getDate()&&IsMonthEquals(date1,date2);
}
function GetMonthDayCount(date)
{
	switch(date.getMonth()+1)
	{
		case 1:case 3:case 5:case 7:case 8:case 10:case 12:
			return 31;
		case 4:case 6:case 9:case 11:
			return 30;
	}
	//feb:
	date=new Date(date);
	var lastd=28;
	date.setDate(29);
	while(date.getMonth()==1)
	{
		lastd++;
		AddDays(date,1);
	}
	return lastd;
}


function GetHarfYear(date)
{
	var v=date.getYear();
	if(v>9)return v.toString();
	return "0"+v;
}
function GetFullMonth(date)
{
	var v=date.getMonth()+1;
	if(v>9)return v.toString();
	return "0"+v;
}
function GetFullDate(date)
{
	var v=date.getDate();
	if(v>9)return v.toString();
	return "0"+v;
}
function Replace(str,from,to)
{
	return str.split(from).join(to);
}
function FormatDate(date,str)
{
	str=Replace(str,"yyyy",date.getFullYear());
	str=Replace(str,"MM",GetFullMonth(date));
	str=Replace(str,"dd",GetFullDate(date));
	str=Replace(str,"yy",GetHarfYear(date));
	str=Replace(str,"M",date.getMonth()+1);
	str=Replace(str,"d",date.getDate());
	return str;
}
function ConvertDate(str)
{
	str=(str+"").replace(/^\s*/g,"").replace(/\s*$/g,"");
	var d;
	if(/^[0-9]{8}$/.test(str))	// 20040226 -> 2004-02-26
	{
		d=new Date(new Number(str.substr(0,4)),new Number(str.substr(4,2))-1,new Number(str.substr(6,2)));
		if(d.getTime())return d;
	}
	d=new Date(str);
	if(d.getTime())return d;
	d=new Date(Replace(str,"-","/"));
	if(d.getTime())return d;
	return null;
}

function AddOption(sel,n,v)
{
	var option=win.document.createElement('OPTION');
	option.text=n;
	option.value=v;
	sel.options.add(option);
}


var weeknames=new Array('sun', 'mon', 'tue', 'wed', 'thu', 'fri', 'sat');

var win=window;

var tablepanel;
var floatpanel;
var showed=false;
var date=new Date();
var uidate=null;

var dprev;
var dnext;
var mprev;
var mnext;
var yprev;
var ynext;
var dsel;
var msel;
var ysel;
var tabledays;
var weeksun;
var weekmon;
var weektue;
var weekwed;
var weekthu;
var weekfri;
var weeksat;
var btntd;
var btnok;
var btnbl;

function GetAndClearId(id)
{
	var element=win.document.getElementById(id);
	element.id="";
	return element
}
function Initialize()
{
	tablepanel=GetAndClearId("csdpid_tablepanel");
	dprev=GetAndClearId('csdpid_dprev');
	dnext=GetAndClearId('csdpid_dnext');
	mprev=GetAndClearId('csdpid_mprev');
	mnext=GetAndClearId('csdpid_mnext');
	yprev=GetAndClearId('csdpid_yprev');
	ynext=GetAndClearId('csdpid_ynext');
	dsel=GetAndClearId('csdpid_dsel');
	msel=GetAndClearId('csdpid_msel');
	ysel=GetAndClearId('csdpid_ysel');
	tabledays=GetAndClearId('csdpid_tabledays');
	weeksun=GetAndClearId('csdpid_weeksun');
	weekmon=GetAndClearId('csdpid_weekmon');
	weektue=GetAndClearId('csdpid_weektue');
	weekwed=GetAndClearId('csdpid_weekwed');
	weekthu=GetAndClearId('csdpid_weekthu');
	weekfri=GetAndClearId('csdpid_weekfri');
	weeksat=GetAndClearId('csdpid_weeksat');
	btntd=GetAndClearId('csdpid_btntd');
	btnok=GetAndClearId('csdpid_btnok');
	btnbl=GetAndClearId('csdpid_btnbl');
	
	if(document.addEventListener)
	{
		mprev.innerHTML="&lt";
		mnext.innerHTML="&gt";
		dprev.innerHTML="&lt";
		dnext.innerHTML="&gt";
		yprev.innerHTML="&lt";
		ynext.innerHTML="&gt";
	}
	
	Init_Elements();
}
function Init_Elements()
{
	AttachEvent(tablepanel,'onkeydown',function tablepanel_onkeydown(){
		var r=HandleKeyDown(win.event);
		if(r)
		{
			return win.event.returnValue=false;
		}
	});

	if(element.getAttribute("Nullable")!='True')
		btnbl.style.display='none';
		
	for(var i=0;i<12;i++)msel.options[i].value=i+1;
	dsel.options.length=0;
	for(var i=1;i<=28;i++)
		AddOption(dsel,i,i);
	ysel.options.length=0;
	var thisyear=new Date().getFullYear();
	for(var i=thisyear-20;i<thisyear+20;i++)AddOption(ysel,i,i);
	ysel.value=new Date().getFullYear();
	
	tablepanel.onselectstart=function()
	{
		return win.event.returnValue=false;
	}
	
	var labels=[dprev,dnext,mprev,mnext,yprev,ynext,weeksun,weekmon,weektue,weekwed,weekthu,weekfri,weeksat];

	for(var i=0;i<labels.length;i++)
	{
		if(labels[i].style.disabled!='1')
		{
			labels[i].style.cursor='hand';
			labels[i].onmouseover=onlabel_mouseover;
			labels[i].onmouseout=onlabel_mouseout;
		}
	}
	
	msel.onchange=dsel.onchange=ysel.onchange=
	function FromUIDateOfSelect()
	{
		date=new Date(ysel.value,msel.selectedIndex,dsel.selectedIndex+1);
		UpdateAndSync();
	}
	
	var weeknums=[weeksun,weekmon,weektue,weekwed,weekthu,weekfri,weeksat];
	for(var i=0;i<weeknums.length;i++)
	{
		if(weeknums[i].style.disabled!='1')
			weeknums[i].onclick=onweeknum_click;
	}

	dprev.onclick=dprev.ondblclick=function dprev_onclick(){ AddD(-1); }
	dnext.onclick=dnext.ondblclick=function dnext_onclick(){ AddD(1); }
	
	mprev.onclick=mprev.ondblclick=function mprev_onclick(){ AddM(-1); }
	mnext.onclick=mnext.ondblclick=function mnext_onclick(){ AddM(1); }
	
	yprev.onclick=yprev.ondblclick=function yprev_onclick(){ AddY(-1); }
	ynext.onclick=ynext.ondblclick=function ynext_onclick(){ AddY(1); }
	
	btntd.onclick=function()
	{
		date=new Date();
		UpdateAndSync();
	}
	btnbl.onclick=function()
	{
		SetElementValue("");
		HidePanel();
	}
	btnok.onclick=function()
	{
		SyncToElement();
		HidePanel();
	}
	
	floatpanel.style.position='absolute';
	floatpanel.style.left=0;
	floatpanel.style.top=0;
	floatpanel.style.display='none';
	floatpanel.style.zIndex=10000;
	
	switch(element.getAttribute("PopupMode"))
	{
		case "ondblclick":
			AttachEvent(element,'ondblclick',element_onpopup);
			break;
		case "oncontextmenu":
			AttachEvent(element,'oncontextmenu',element_onpopup);
			break;
		case "onclick":
		default:
			AttachEvent(element,'onclick',element_onpopup);
			break;
	}

	function element_onpopup(event)
	{
		ShowPanel();
		event=window.event||event;
		if(event.type=='contextmenu')
		{
			if(event.preventDefault)event.preventDefault();
			return event.returnValue=false;
		}
	}
	
}

function AddD(d)
{
	AddDays(date,d)
	UpdateAndSync();
}
function AddM(m)
{
	AddMonths(date,m)
	UpdateAndSync();
}
function AddY(y)
{
	AddYears(date,y)
	UpdateAndSync();
}


function UpdateAndSync()
{
	UpdateUI();
	SyncToElement();
}

function SetWrong()
{
	element.runtimeStyle.backgroundColor=element.getAttribute("WrongBackColor")||'#ffffff'
	element.runtimeStyle.color=element.getAttribute("WrongForeColor")||'red'
}
function SetRight()
{
	element.runtimeStyle.backgroundColor=''
	element.runtimeStyle.color=''
}

var settingbythis=false;
function SetElementValue(str)
{
	settingbythis=true;
	try
	{
		element.value=str;
		//if(element.fireEvent)
		//{
		//	//element.fireEvent('onchange');
		//}
		//else if(element.onclick)
		//{
		//	element.onclick();
		//}
		
	}
	finally
	{
		settingbythis=false;
	}
}
function GetElementValue()
{
	return element.value.replace(/^\s*/g,"").replace(/\s*$/g,"");
}

function SyncToElement()
{
	SetRight();
	SetElementValue(FormatDate(date,element.getAttribute("DateFormatString")||"yyyy-MM-dd"));
	if(element.select)element.select();
}
function SyncFromElement()
{
	var value=GetElementValue();
	if(element.getAttribute("Nullable")=='True'&&value=='')
	{
		SetRight();
		return;
	}
	var d=ConvertDate(value);
	if(d)
	{
		date=d;
		if(showed)
			UpdateUI();
		SetRight();
		return;
	}
	SetWrong();
}

function UpdateUI()
{
	if(uidate&&IsDateEquals(date,uidate))return;
	uidate=new Date(date);
	
	if(msel.selectedIndex!=date.getMonth())
		msel.selectedIndex=date.getMonth();
	var dc=GetMonthDayCount(date);
	if(dsel.options.length>dc)
	{
		dsel.options.length=dc;
	}
	else
	{
		for(var i=dsel.options.length;i<dc;i++)
			AddOption(dsel,i+1,i+1);
	}
	if(dsel.selectedIndex!=date.getDate()-1)
	{
		dsel.selectedIndex=date.getDate()-1;
	}
	var ymiddle=parseInt(ysel.options.item(20).value);
	if(date.getFullYear()>ymiddle+8||date.getFullYear()<ymiddle-8)
	{
		ymiddle=date.getFullYear();
		ysel.options.length=0;
		for(var i=ymiddle-20;i<ymiddle+20;i++)
		{
			AddOption(ysel,i,i);
		}
		ysel.value=ymiddle;
	}
	else
	{
		if(ysel.value!=date.getFullYear())
			ysel.value=date.getFullYear();
	}
	
	if(document.addEventListener)
	{
		var oldtabledays=tabledays;
		tabledays=oldtabledays.cloneNode(true);
		
		CreateDays();
		
		var p=oldtabledays.parentNode;
		p.removeChild(oldtabledays);
		p.appendChild(tabledays);
	}
	else
	{
		CreateDays();
	}
	
}

function CreateDays()
{
	while(tabledays.rows.length>1)tabledays.deleteRow(1);
	var eachday=new Date(date.getFullYear(),date.getMonth(),1);
	AddDays(eachday,-eachday.getDay());
	var arr=[];
	do
	{
		var wds=[];
		for(var i=0;i<7;i++)
		{
			wds[i]=new Date(eachday);
			AddDays(eachday,1)
		}
		arr[arr.length]=wds;
	}
	while(eachday.getYear()==date.getYear()&&eachday.getMonth()==date.getMonth());
	
	for(var t=0;t<arr.length;t++)
	{
		var tr=tabledays.insertRow(-1);
		var wds=arr[t];
		for(var i=0;i<wds.length;i++)
		{
			var td=tr.insertCell(-1);
			td.style.textAlign='center';
			
			var wd=wds[i];
			
			var classnames=["csdpdate","csdpweek"+weeknames[i],"csdp"+FormatDate(wd,"yyyyMMdd")];
			
			if(IsMonthEquals(wd,date))
			{
				classnames[classnames.length]="csdpthismonth";
				
				if(IsToday(wd))
				{
					classnames[classnames.length]="csdptoday";
				}
			
				if(IsDateEquals(wd,date))
				{
					classnames[classnames.length]="csdpcurrent";
					
					td.style.textDecoration='underline';
					td.style.backgroundColor=element.getAttribute("CurrentBackColor")||'#426FD9'
					td.style.color=element.getAttribute("CurrentForeColor")||'white';
				}
				else if(IsToday(wd))
				{
					td.style.backgroundColor=element.getAttribute("TodayBackColor")||'#426FD9'
					td.style.color=element.getAttribute("TodayForeColor")||'white';
				}
				else
				{
					td.style.backgroundColor=element.getAttribute("MonthBackColor")||'';
					td.style.color=element.getAttribute("MonthForeColor")||'#0066cc';	
					
				}
			}
			else
			{
				classnames[classnames.length]="csdpothermonth";
				
				td.style.backgroundColor=element.getAttribute("OtherBackColor")||'#cccccc';
				td.style.color=element.getAttribute("OtherForeColor")||'#606060';
				
			}
			
			td.className=classnames.join(' ');

			td.date=wd;
			td.innerHTML=wd.getDate();
			
			if(td.style.disabled!='1')
			{
				td.style.cursor='hand';
				td.onmouseover=onlabel_mouseover;
				td.onmouseout=onlabel_mouseout;
				td.onclick=ondate_click;
			}
			else
			{
				td.disabled=true;
			}
		}
	}
}

function onlabel_mouseover(event)
{
	event=win.event||event;
	var e=event.srcElement||event.target;
	if(e.getAttribute("_isover")!="1")
	{
		e.setAttribute("_isover","1");
		e.setAttribute("_color",e.style.color);
	}
	e.style.color=element.getAttribute("OverColor")||'firebrick'
}
function onlabel_mouseout(event)
{
	event=win.event||event;
	var e=event.srcElement||event.target;
	e.setAttribute("_isover","");
	e.style.color=e.getAttribute("_color");
}

function onweeknum_click(event)
{
	event=win.event||event;
	var e=event.srcElement||event.target;
	var num=parseInt(e.getAttribute("weeknum"));
	AddDays(date,num-date.getDay());
	UpdateAndSync();
}

function ondate_click(event)
{
	event=win.event||event;
	var e=event.srcElement||event.target;
	var thedate=e.date;
	if(IsMonthEquals(date,thedate))
	{
		date=thedate;
		SyncToElement();
		HidePanel();
		if(element.ondayclick)element.ondayclick(date);
	}
	else
	{
		date=thedate;
		UpdateAndSync();
	}
	
}

function HandleKeyDown(event)
{
	switch(event.keyCode)
	{
		case 13:
			SyncToElement();
			HidePanel();
			return true;
		case 38:
		case 40:
			var d=ConvertDate(GetElementValue());
			if(d)
			{
				AddDays(d,event.keyCode-39);
				date=d;
				SyncToElement();
				if(showed)
					UpdateUI();
			}
			return true;
		case 33:
		case 34:
			var d=ConvertDate(GetElementValue());
			if(d)
			{
				AddMonths(d,event.keyCode*2-33*2-1);
				date=d;
				SyncToElement();
				if(showed)
					UpdateUI();
			}
			return true;
	}
}


//	this window
function ShowPanel()
{
	if(showed)return;
	
	if(element.readOnly)return;
	
	showed=true;
	
	UpdateUI();
	
	AttachEvent(document,'onmousedown',document_onmousedown);

	floatpanel.style.display='';
	
	var pos=CalcPosition(floatpanel,element);
	pos.top=pos.top+element.offsetHeight;
	
	AdjustMirror(floatpanel,element,pos);
	
	floatpanel.style.left=pos.left+"px";
	floatpanel.style.top=pos.top+"px";
}
function HidePanel()
{
	if(!showed)return;
	showed=false;
	DetachEvent(document,'onmousedown',document_onmousedown);
	
	floatpanel.style.left=0;
	floatpanel.style.top=0;
	floatpanel.style.display='none';
}
function document_onmousedown(event)	
{
	event=win.event||event;	
	var e=event.srcElement||event.target;
	if(_contains(element,e))return;
	if(_contains(tablepanel,e))return;
	HidePanel();
}

AttachEvent(element,'onkeydown',function element_onkeydown(event){
	event=win.event||event;	
	if(HandleKeyDown(event))
	{
		return event.returnValue=false;
	}
});

AttachEvent(element,'onpropertychange',function element_onpropertychange(){	
	if(settingbythis)return;
	if(event.propertyName=='value')
		SyncFromElement();
});

if(element.getAttribute("UseFrame")=='False')
{
	var div=document.createElement("DIV");
	div.innerHTML=inithtml;
	var panel=div.firstChild;
	floatpanel=panel;
	window.document.body.insertBefore(panel,window.document.body.firstChild);
	win=window;
	Initialize();
	SyncFromElement();
	return;
}

var iframe=document.createElement("IFRAME");
iframe.frameBorder=0;
iframe.src="about:blank";
iframe.style.display="none";


window.document.body.insertBefore(iframe,window.document.body.firstChild);

if(iframe.contentWindow)	//ie5.5
{
	win=iframe.contentWindow;
}
else
{	
	win=window.open(element.getAttribute("FrameSrc")||'about:blank',iframe.name);
}
var intervalid=window.setInterval(handlereadystatechange,100)

function handlereadystatechange()
{
	if(!win.document.body)
	{
		return;
	}
	
	window.clearInterval(intervalid);
	
	//TODO:cssText not worked for Opera
	win.document.body.style.backgroundColor="blue";
	win.document.body.style.border="0px";
	win.document.body.style.margin="0px";
	win.document.body.style.padding="0px";
	win.document.body.style.overflow="hidden";
	win.document.body.scroll="no";
	win.document.body.innerHTML=inithtml;
	
	floatpanel=iframe;
	
	iframe.style.width="284px";
	iframe.style.height="240px";
	
	Initialize();	
	SyncFromElement();
}

element.popupDatePicker=ShowPanel;


}	//function CuteSoftDatePickerInitialize(element)

