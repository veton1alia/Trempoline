
////////////////////////////////////////////////////////////////////////
//JQUERY extends begin
////////////////////////////////////////////////////////////////////////
$.fn.extend({

    serializeOBJ: function () { //serializes form elements to object

        var object = {};
        var jq = $(this);

        $(this).find("input, textarea, select").each(function (key, value) {

            var $el = $(this);


            if ($el.is('input[type="file"]')) { // type file clone
                object[$el.attr("name")] = $el.clone(true).removeAttr('id'); // remove id we can't have elements with the same id 
            }
            else {
                var val = $el.val().replace(/"/g, '&quot;'); //in case there are quotes inside a textarea or input

                if ($el.is(':checkbox')) {
                    object[$el.attr("name")] = $el.is(':checked');
                }
                else if ($el.is('select')) {
                    object[$el.attr("name")] = {
                        value: $el.find(":selected").val(),
                        text: $el.find(":selected").text().trim()
                    }
                }
                else {
                    object[$el.attr("name")] = val;
                }
            }


        });

        return object;
    },

    bindToObject: function (object) {

        for (var prop in object) {

            if (object.hasOwnProperty(prop)) {

                if (typeof object[prop] !== 'object') { // if not jquery object inside or file input
                    var isBoolean = ['true', 'false'].indexOf(object[prop].toLowerCase()) > -1;

                    if (isBoolean) {
                        $(this).find('input[name="' + prop + '"]').attr('checked', object[prop]);

                    } else {

                        $(this).find('[name="' + prop + '"]').val(object[prop])
                    }
                }
            }
        }
    }
});
////////////////////////////////////////////////////////////////////////
//JQUERY EXTENDS END
////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////
//TABITEM begin
////////////////////////////////////////////////////////////////////////
var TabItem = (function ($) {

    function to(url) {
        var $tab = $("#tab");

        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'html', // <-- to expect an html response,
            cache: true,
            success: function (tabData) {
                $tab.html(tabData);
            },
            error: function (error) {

                $("#tab").text(error.responseText);
            }
        });
    }

    function clickHandler(element) {
        $(element).click(function () {

            var menu = $(this).attr("data-menu");

            //switch (menu) {
            //    case "Contact": to("/Contact/" + editOrCreate()); return;
            //    case "Attachement": to("/Attachement/Create/" + route().id); return;
            //    default: return;
            //}

            if (menu === "Contact") to("/Contact/" + editOrCreate());
            else {
                to("/" + menu + "/Create/" + route().id);
            }

            window.location.hash = "#tab=" + menu;
            

        });
    }

    function editOrCreate() {
        var r = route();
        return r.edit ? "Edit/" + r.id : "Create";
    }

    /*   function loader() {
           $(document).ajaxStart(function () {
               $("#tab #loader").show();//show loader before tab appears
           });
       }
       */
    function route() {
        var p = window.location.pathname;
        var tab = window.location.hash;

        if (p.indexOf("Beneficiary/Detail") > -1) {
            var data = p.substring(1).split('/');

            if (data[2]) {
                return {
                    edit: true,
                    id: data[2],
                    tab : tab !== '' ? tab.split('=')[1] : undefined
                }
            }

            return { edit: false }
        }
    }

    return {

        init: function () {
            var r = route();

            if (r.edit && r.tab !== undefined)
            {
                to("/" + r.tab + "/Create/" + r.id + "#tab=" + r.tab);
                $('a[data-menu=Contact]').parent().removeClass('active');
                $("a[data-menu=" + r.tab + "]").parent().addClass('active');
            }
            else if (r.edit) {
                to("/Contact/Edit/" + r.id);
            } else {
                to("/Contact/Create");
            }

            

            clickHandler(".tabbable ul li a");
        },

        to: to
    }

})(jQuery);

////////////////////////////////////////////////////////////////////////
//TABITEM end
////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////
//Contact object begin
////////////////////////////////////////////////////////////////////////

var Common = {
    //function that recreates indexes of hidden inputs found in table rows such as Addresses[0].Property, ...
    recreateIndexes: function (target) {
        $(target).each(function (key, value) {

            var $el = $(this);

            $el.find('td input').each(function () {

                var $input = $(this);
                var attr = $input.attr('name').replace(/\[[0-9]{1,}\]/g, '[' + key + ']');
                $input.attr('name', attr);

            });

        });
    },

    inputDisabled: function (type, name, value) {
        return '<input type="' + type + '" name="' + name + '" value="' + value + '" class="textInput" disabled/>'
    },

    objectGetPropertiesOnly: function (object) {
        var obj = {};

        for (var prop in object) {
            if (object.hasOwnProperty(prop)) {
                var newProp = prop.substring(prop.indexOf('].') + 2);
                obj[newProp] = object[prop];
            }
        }
        return obj;
    },

    confirm : function (heading, body, cancelButtonTxt, okButtonTxt, callback) {
    
        var confirmModal = $('<div id="light">' +
                                 '<header id="heading">' 
                                   + heading +
                                 '</header>' +
                                    '<hr />' +
                                 '<section id="body">' 
                                    + body +
                                '</section>' +
                                    '<hr />' +
                                '<footer>' +
                                '   <button id = "okButton" class="btn btn-default">' + okButtonTxt + '</button>' +
                                '   <button id = "cancelButton" class="btn btn-default">' + cancelButtonTxt + '</button>' +
                                '</footer>' +
                            '</div>').appendTo($('html'));
        
        var fade = $('<div id="fade"></div>').click(function () {

            confirmModal.hide();
            $(this).hide();

        }).appendTo($('html'));

        function show()
        {
            fade.show();
            confirmModal.show();
        }

        function hide() {
            fade.hide();
            confirmModal.hide();
        }

        confirmModal.find('#okButton').click(function (event) {
            callback();
            hide();
        });

        confirmModal.find('#cancelButton').click(function (event) {
            hide();
        });

        show();
    }
}


var Contact = (function ($) {

    var $beneficiaryForm, $addressAddOrEditBtn = null;

    function addAddress() {

        $addressAddOrEditBtn.click(function () {

            var $addressForm = $("#address_form");
            var valid = $addressForm.valid();

            if (valid) {

                var adr = $addressForm.serializeOBJ();
                var $adrTable = $("#address_table");
                var index = $adrTable.find('tbody tr').length;

                $adrTable.append(
                        '<tr>' +
                            '<td>' + Common.inputDisabled('text', 'Addresses[' + index + '].Ligne1', adr.Ligne1) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Addresses[' + index + '].Ligne2', adr.Ligne2) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Localities[' + index + '].LocaliteNom', adr.LocaliteNom) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Localities[' + index + '].CodePostal', adr.CodePostal) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Addresses[' + index + '].Pays', adr.Pays) + '</td>' +
                            '<td>' + adr.IDTypeAdresse.text +
                                Common.inputDisabled('hidden', 'Addresses[' + index + '].IDTypeAdresse', adr.IDTypeAdresse.value)
                            + '</td>' +
                        '</tr>'
                    );

                if ($addressAddOrEditBtn.text() === 'Sauver') {
                    $adrTable.find('tbody tr.selectedRow').remove();
                    recreateIndexes('#address_table tbody tr');
                }

                $("#AddressModal").modal('hide');

            }
        });
    }

    function editAddress() {
        $('#benef_address_edit').click(function () {

            var object = $('#address_table tbody tr.selectedRow').serializeOBJ();
            if (object != null)
                $('#address_form').bindToObject(Common.objectGetPropertiesOnly(object));

        });
    }

    function addressFormModal() //reset form
    {

        $('#AddressModal').on('hide.bs.modal', function (e) {
            $('#address_form').trigger('reset');
        });

        $('#AddressModal').on('show.bs.modal', function (e) {

            var triggerTargetID = $(e.relatedTarget).attr('id');

            if (triggerTargetID === 'benef_address_edit') {
                $addressAddOrEditBtn.text("Sauver");
            }
            else {
                $addressAddOrEditBtn.text("Ajouter");
            }
        });

    }

    function addStay() {
        $('#benef_stay_add, div#edit_stay_btn_container button#update_stay').click(function () {

            var $stayForm = $('#stay_form');
            var valid = $stayForm.valid();

            if (valid) {
                var stay = $stayForm.serializeOBJ();
                var $tab = $("#stays_table tbody");
                var i = $tab.find('tr').length;

                $tab.append(
                        '<tr>' +
                            '<td>' + Common.inputDisabled('text', 'Stays[' + i + '].DateEntree', stay.DateEntree) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Stays[' + i + '].DateSortie', stay.DateSortie) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Stays[' + i + '].DateDebutConvention', stay.DateDebutConvention) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Stays[' + i + '].DateDebutAccord', stay.DateDebutAccord) + '</td>' +
                            '<td>' + Common.inputDisabled('text', 'Stays[' + i + '].DateFinAccord', stay.DateFinAccord) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].HeureEntree', stay.DateEntree + ' ' + stay.HeureEntree) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].HeureSortie', stay.DateSortie + ' ' + stay.HeureSortie) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].DateFinConvention', stay.DateFinConvention) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].ReferenceConvention', stay.ReferenceConvention) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].Prolongation', stay.Prolongation) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].DateFinProlongation', stay.DateFinProlongation) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].EtatAccordMutualiste', stay.EtatAccordMutualiste) + '</td>' +
                            '<td class="displayNone">' + Common.inputDisabled('hidden', 'Stays[' + i + '].Commentaires', stay.Commentaires) + '</td>' +
                        '</tr>'

                )
                if ($(this).attr('id') === 'update_stay') {
                    $tab.find('tr.selectedRow').remove();
                    Common.recreateIndexes("#stays_table tbody tr");
                    $('div#edit_stay_btn_container').hide();
                    $stayForm.trigger('reset');
                }
            }

        });
    }

    function editStay() {
        $('body').on('click', 'table#stays_table tbody tr', function () {

            var $el = $(this);
            var object = $el.serializeOBJ();

            $("#stay_form").bindToObject(Common.objectGetPropertiesOnly(object));
        });

    }

    function setUpValidationForAddressForm() {

        $("#address_form").validate({
            rules: {
                Ligne1: { required: true },
                LocaliteNom: { required: true },
                CodePostal: {
                    required: true,
                    number: true
                },
                Pays: { required: true },
                IDTypeAdresse: { required: true }
            },
            messages: {
                Ligne1: { required: "L'adresse est obligatoire" },
                LocaliteNom: { required: "La localite est obligatoire" },
                CodePostal: {
                    required: "Le code postal est obligatoire",
                    number: "Le code postal doit être en chiffre"
                },
                Pays: { required: "Le pays est obligatoire" },
                IDTypeAdresse: { required: "Veuillez choisir un type d'adresse" }
            }
        });

    }

    function setUpValidationStayForm() {
        $('#stay_form').validate({
            rules: {
                DateEntree: { required: true },
                HeureEntree: { required: true },
                DateDebutConvention: { required: true },
                DateFinConvention: { required: true },
                ReferenceConvention: { required: true }
            },
            messages: {
                DateEntree: { required: "La date d'entrée est obligatoire" },
                HeureEntree: { required: "L'heure d'entrée est obligatoire" },
                DateDebutConvention: { required: "La date de début de convention est obligatoire" },
                DateFinConvention: { required: "La date de fin de convention  est obligatoire" },
                ReferenceConvention: { required: "La réference de convention est obligatoire" }
            }
        });
    }

    function setUpBeneficiaryValidationForm() {
        $beneficiaryForm.validate({

            rules: {
                'Beneficiary.Nom': { required: true, maxlength: 100 },
                'Beneficiary.Prenom': { required: true, maxlength: 100 },
                'Beneficiary.DateNaissance': { required: true },
                'Beneficiary.CarteIdentite': { required: true, number: true, maxlength: 100 }
            },

            messages: {
                'Beneficiary.Nom': { required: "Le nom est obligatoire", maxlength: "longueur maximum 100 caratères" },
                'Beneficiary.Prenom': { required: "Le prenom est obligatoire", maxlength: "longueur maximum 100 caratères" },
                'Beneficiary.DateNaissance': { required: "Le date de naissance est obligatoire" },
                'Beneficiary.CarteIdentite': { required: "Le numéro de carte d'identité est obligatoire", number: "composé uniquement des chiffres", maxlength: "longueur maximum 100 caratères" }
            }
        });
    }

    function selectRow() {
        $('body').on('click', 'table#address_table tbody tr, table#stays_table tbody tr', function () {

            var $el = $(this);
            $el.parent().find('tr, td input').removeClass('selectedRow');
            $el.addClass('selectedRow');

            if ($el.parent().parent().attr('id') === 'stays_table') {
                $('div#edit_stay_btn_container').show();
            }
        });
    }

    function deleteRow() {
        $("#benef_address_delete,#benef_stay_delete").click(function () {

            var id = $(this).attr('id');
            if (id === 'benef_address_delete') {
                $("table#address_table tbody tr.selectedRow").remove();
                Common.recreateIndexes("table#address_table tbody tr");
            }
            else if (id === 'benef_stay_delete') {
                $("table#stays_table tbody tr.selectedRow").remove();
                Common.recreateIndexes("table#stays_table tbody tr");
            }

        });
    }

    function submitForm() {
        $('#saveContact').click(function () {

            if ($beneficiaryForm.valid()) {
                $beneficiaryForm.find('input[disabled]').removeAttr('disabled');
                $beneficiaryForm.submit();
            }

        });
    }


    return {
        init: function () {
            $addressAddOrEditBtn = $("#addressAddOrEdit");
            $beneficiaryForm = $('#BeneficiaryContact');
            addAddress();
            editAddress();
            addressFormModal();
            setUpValidationForAddressForm();
            setUpBeneficiaryValidationForm();
            setUpValidationStayForm();
            selectRow();
            deleteRow();
            addStay();
            editStay();
            submitForm();

        }
    }

})(jQuery);

////////////////////////////////////////////////////////////////////////
//Contact object end
////////////////////////////////////////////////////////////////////////


var Attachement = (function ($) {

    function selectRow() {

        var clicks = 0;

        $('table#attach_table').on('click', 'tbody tr', function (e) {

            // clicks += 1;

            var $el = $(this);
            $form = $('#attach_form');

            if ($el.hasClass('selectedRow')) {

                $el.removeClass('selectedRow');
                $form = $('#attach_form').trigger('reset');
                $('#download').hide();
                $('#upload').show();

            } else {

                $el.parent().find('tr').removeClass('selectedRow');
                $el.addClass('selectedRow');

                var obj = Common.objectGetPropertiesOnly($el.serializeOBJ());
                $form.bindToObject(obj);
                changeDownloadUrl($('#IDBeneficiaire').val(), obj.IDPieceJointe);
                $('#download').show();
                $('#upload').hide();
            }

        });
    }

    function changeDownloadUrl(benefId, fileId) {
        var href = $('#download').attr('href');
        var newHref = href.substring(0, href.indexOf('?') + 1) + 'benefId=' + benefId + '&fileId=' + fileId;
        $('#download').attr('href', newHref);

    }

    function deleteRow() {

        $("#attach_delete").click(function () {

            var $selected = $("table#attach_table tbody tr.selectedRow");
            var $el = $selected.find('td.displayNone input[name$="IDPieceJointe"]');

            if ($el[0] !== undefined) {
                var $from = $("#attachements_form");
                var index = $from.find('input[name^="AttachementsToDelete"]').length;
                console.log(index);
                $from.append(Common.inputDisabled('hidden', 'AttachementsToDelete[' + index + ']', $el.val()));
            }

            $("table#attach_table tbody tr.selectedRow").remove();
            Common.recreateIndexes("table#attach_table tbody tr");
            $form = $('#attach_form').trigger('reset');
            $('#download').hide();
            $('#upload').show();
        });

    }

    function setUpValidationForm() {
        $('#attach_form').validate({

            rules: {
                LibellePiece: { required: true },
                DatePiece: { required: true },
                Piece: { required: true }
            },

            messages: {
                LibellePiece: { required: "Le Libelle est obligatoire" },
                DatePiece: { required: "La date est obigatoire" },
                Piece: { required: "Le fichier est obligatoire" }
            }

        }).settings.ignore = []; // don't ignore hidden elements
    }

    function addAttachement() {
        $("#attach_add, #edit").click(function () {

            var $form = $('#attach_form');
            var edit = $(this).attr('id') === 'edit';

            if ($form.valid()) {

                var obj = $form.serializeOBJ();
                var index = $('#attach_table tbody tr').length;
                var $tab = $('#attach_table tbody');

                $('#attach_table tbody').append(
                        '<tr>' +
                               '<td>' + Common.inputDisabled('text', 'Attachements[' + index + '].LibellePiece', obj.LibellePiece) + '</td>' +
                               '<td>' + Common.inputDisabled('text', 'Attachements[' + index + '].DatePiece', obj.DatePiece) + '</td>' +
                               '<td>' + obj.IDTypeAction.text +
                                    Common.inputDisabled('hidden', 'Attachements[' + index + '].IDTypeAction', obj.IDTypeAction.value) + '</td>' +
                                '<td><span class="glyphicon glyphicon-file"></span><div class="displayNone"></di></td>' +
                        '</tr>'
                    );

                obj.Piece.attr('name', 'Attachements[' + index + '].Piece');
                $('#attach_table tbody tr:last td:last div').html(obj.Piece);
            }
        });
    }

    function upload() {
        $("#upload").click(function () {
            $('#Piece').trigger('click');
        });

        $("#Piece").change(function () {
            $("#fileName").text($('#Piece')[0].files[0].name);
        });
    }

    function search() {
        $("#search_btn").click(function () {

            TabItem.to("/Attachement/Create?" + $('#search_form').serialize());

        });
    }

    function submitAttachements() {
        $('#saveAttachement').click(function () {

            var $from = $('#attachements_form');

            $from.find('input[disabled]').removeAttr('disabled');
            $from.submit();
        });
    }

    return {
        init: function () {
            selectRow();
            deleteRow();
            addAttachement();
            upload();
            search();
            setUpValidationForm();
            submitAttachements();
        }
    }

})(jQuery);

var SearchBeneficiaryDataTables = (function ($) {

    var rowSelected = false;

    function selectRow() {

        $('#BeneficiaryTable tbody').on('click', 'tr', function () {
            var $el = $(this);
            if ($el.hasClass('selectedRow')) {
                $el.removeClass('selectedRow');
                rowSelected = false;
            }
            else {
                $('#BeneficiaryTable tbody tr.selectedRow').removeClass('selectedRow');
                $el.addClass('selectedRow');
                rowSelected = true;
                constructUrl();
            }
        });
    }

    function constructUrl() {
        var links = ["Detail", "Delete"];
        var id = $('#BeneficiaryTable tbody tr.selectedRow').attr('data-id');

        links.forEach(function (link) {
            var el = $('a#' + link);
            var href = el.attr('href');
            el.attr('href', href.substring(0, href.indexOf(link) + link.length) + '/' + id);
        });

    }

    function deleteOrDetails() {
        $("#Detail,#Delete").click(function (e) {

            if (!rowSelected) {
                e.preventDefault();
                alert("Aucune ligne n'a été sélectionnée");

            } else {

                if ($(this).attr('id') === "Detail") {
                    return;

                } else {
                    e.preventDefault();
                    var scope = this;
                    Common.confirm("Confirmer", "Etes-vous sure de supprimer?", "annuler", "confimer", function () {

                            $.ajax({
                                method: "POST",
                                url: $(scope).attr('href'),
                                success: function (data) {
                                    if (data === "OK") {
                                        $('#BeneficiaryTable tbody tr.selectedRow').remove();
                                    } else {
                                        console.log(data);
                                    }
                                }
                            });
                    });
                }
            }

        });
    }

    return {
        init: function () {
            //   $('#BeneficiaryTable').dataTable();
            selectRow();
            deleteOrDetails();
        }
    }

})(jQuery);

var Earning = (function ($) {

    function setupValidation()
    {
        $("#earnings_form").validate({
            rules: {
                RevenusCPAS: { number: true },
                RevenusIITMutuelle: { number: true },
                RevenusAllocHandic: { number: true },
                MontantPensionAlim: { number: true },
                AutresRevenus:      { number: true },
            },
            messages: {
                RevenusCPAS: { number: "ce champ doîte être un nombre" },
                RevenusIITMutuelle: { number: "ce champ doîte être un nombre" },
                RevenusAllocHandic: { number: "ce champ doîte être un nombre" },
                MontantPensionAlim: { number: "ce champ doîte être un nombre" },
                AutresRevenus: { number: "ce champ doîte être u nombre" },
            }
        });

        $("#earnings_form").submit(function (e) {

            if(!$(this).valid)
            {
                e.preventDefault();
            }

        });
    }

    function total() {
        $("#earnings_form input[data-total]").focusout(function () {
            var total = 0;
            $("#earnings_form input[data-total]").each(function (key, value) {
                // console.log($(this).val());
                var val = $(this).val();
                if(val !== '')
                    total += parseFloat($(this).val());
            });

            $('#TotalRevenus[name="TotalRevenus"]').val(total);
        });
    }
    return {
        init: function () {
            setupValidation();
            total();
        }
    }

})(jQuery);

var Debt = (function ($) {
    
    function setupFormValidation()
    {
        $("#Debt_form").validate({
            rules: {
                CPAdminBien: { number: true },
                MailAdminBien: { email: true }
            }/*,
            messages: {
                CPAdminBien: { number: "Ce champ doit être un numéro" },
                MailAdminBien: { email: "Format de l'email n'est pas correct" }
            }*/
        });

        $("#Debt_form").submit(function (e) {

            if(!$(this).valid())
            {
                e.preventDefault();
            }
        });
    }

    return {
        init : function()
        {
            setupFormValidation();
        }
    }

})(jQuery);

var Work = (function ($) {

    function setupFormValidation() {
        $("#work_form").validate({
            rules: {
                CPEmployeur: { number: true },
                CPONEM: { number: true },
                CPSyndicat: { number: true },
                MailEmployeur: { email: true },
                MailONEM: { email: true },
                MailSyndicat: { email: true }
            }
        });

        $("#work_form").submit(function (e) {

            if (!$(this).valid()) {
                e.preventDefault();
            }
        });
    }

    return {
        init: function () {
            setupFormValidation();
        }
    }

})(jQuery);

var Finance = (function ($) {

    var $file = null;

    function setupFormValidation() {

        $("#finance_form").validate({
            rules: {
                SoldeCompteVue: { number: true },
                SoldeCompteVue: { number: true }
            }
        });

        $("#finance_form").submit(function (e) {

            if (!$(this).valid()) {
                e.preventDefault();
            }
        });
    }

    function triggerFileUpload()
    {
        $("#uploadBtn").click(function () { $file.click(); });
    }

    function getFileName()
    {
        $file.change(function () {
            $("#fileName").text($file[0].files[0].name);
        });
    }

    return {
        init: function () {
            $file = $("#uploadFile");
            setupFormValidation();
            triggerFileUpload();
            getFileName()
        }
    }

})(jQuery);