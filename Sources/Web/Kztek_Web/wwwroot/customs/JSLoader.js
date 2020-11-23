$(function () {
    JSLoader.init();
});

var JSLoader = {
    init: function () {
        JSLoader.load_Toast();
        JSLoader.load_DateTimePicker();
        JSLoader.load_MaskInput();
        JSLoader.load_ChosenSelect();
        JSLoader.load_MultiSelect();
    },

    load_DateTimePicker: function () {

        //daterangepicker drp
        $('.drp_auto_input').daterangepicker({
            'applyClass': 'btn-sm btn-success',
            'cancelClass': 'btn-sm btn-default',
            autoUpdateInput: true,
            locale: {
                applyLabel: 'Apply',
                cancelLabel: 'Cancel',
                format: 'DD/MM/YYYY'
            },
            singleDatePicker: true,
            showDropdowns: true
        });

        $('.dtrp_auto_input').daterangepicker({
            'applyClass': 'btn-sm btn-success',
            'cancelClass': 'btn-sm btn-default',
            autoUpdateInput: true,
            locale: {
                applyLabel: 'Apply',
                cancelLabel: 'Cancel',
                format: 'DD/MM/YYYY HH:mm'
            },
            singleDatePicker: true,
            showDropdowns: true
        });

        //datepicker
        $(".dp_input").datepicker({
            showOtherMonths: true,
            selectOtherMonths: false,
            format: "DD/MM/YYYY"
        });

        //datepicker
        $(".dtp_input").datetimepicker({
            format: "DD/MM/YYYY HH:mm"
        });

        //timepicker
        $('.tp_input').timepicker({
            minuteStep: 1,
            showMeridian: false,
            disableFocus: true,
            icons: {
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down'
            }
        }).on('focus', function () {
            $(this).timepicker('showWidget');
        });

    },

    load_MaskInput: function () {

        $(".format_money_9digital_input").mask("000.000.000", { reverse: true });

    },

    load_ChosenSelect: function () {

        $('.chosen-select').trigger('chosen:updated');

        if (!ace.vars['touch']) {
            $('.chosen-select').chosen({ allow_single_deselect: true });

            //resize the chosen on window resize
            $(window)
                .off('resize.chosen')
                .on('resize.chosen', function () {
                    $('.chosen-select').each(function () {
                        var $this = $(this);
                        $this.next().css({ 'width': $this.parent().width() });
                    })
                }).trigger('resize.chosen');

            //resize chosen on sidebar collapse/expand
            $(document).on('settings.ace.chosen', function (e, event_name, event_val) {
                if (event_name != 'sidebar_collapsed') return;
                $('.chosen-select').each(function () {
                    var $this = $(this);
                    $this.next().css({ 'width': $this.parent().width() });
                })
            });
        }

    },

    load_MultiSelect: function () {

        $('.multiselect').multiselect({
            enableFiltering: true,
            enableHTML: true,
            nonSelectedText: "-- Lựa chọn --",
            allSelectedText: "Tất cả đã chọn",
            nSelectedText: "Đã chọn",
            numberDisplayed: 1,
            buttonClass: 'btn btn-white btn-primary',
            templates: {
                button: '<button type="button" class="multiselect dropdown-toggle" data-toggle="dropdown"><span class="multiselect-selected-text"></span> &nbsp;<b class="fa fa-caret-down"></b></button>',
                ul: '<ul class="multiselect-container dropdown-menu"></ul>',
                filter: '<li class="multiselect-item filter"><div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input class="form-control multiselect-search" type="text"></div></li>',
                filterClearBtn: '<span class="input-group-btn"><button class="btn btn-default btn-white btn-grey multiselect-clear-filter" type="button"><i class="fa fa-times-circle red2"></i></button></span>',
                li: '<li><a tabindex="0"><label></label></a></li>',
                divider: '<li class="multiselect-item divider"></li>',
                liGroup: '<li class="multiselect-item multiselect-group"><label></label></li>'
            }
        });

    },

    load_Toast: function () {

        //Toast jquery config
        toastr.options = {
            closeButton: false,
            debug: false,
            positionClass: "toast-bottom-right",
            onclick: null,
            showDuration: "300",
            progressBar: false,
            hideDuration: "1000",
            timeOut: "5000",
            extendedTimeOut: "1000",
            showEasing: "swing",
            hideEasing: "linear",
            showMethod: "fadeIn",
            hideMethod: "fadeOut"
        };

    }
};
