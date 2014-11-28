function showLogonDialog(url) {
    
    $("#dlgLogon").load(url,function() {
        $("#dlgLogon").removeData("validator");
        $("#dlgLogon").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("#dlgLogon");
    });
   
    $("#dlgLogon").dialog();
  
}

