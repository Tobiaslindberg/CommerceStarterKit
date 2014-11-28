$(document).ready(function () {    
    $('#FullRegion_EndDate').datepicker({
        format: 'yyyy/mm/dd',
        weekStart:1
    });
    $('#FullRegion_StartDate').datepicker({
        format: 'yyyy/mm/dd',
        weekStart: 1
    });
    $('.timepicker').timepicker({
        showMeridian: false
    });
});
