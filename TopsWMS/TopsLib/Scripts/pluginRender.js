
///插件渲染
$.fn.pluginRender = function (option) {
    var defaults = {
        data: null,
        labelWidth: 80, //JaneHe 应黄总要求缩小间距及样式 20190612 原先100
        moduleConfig: null,
        onBeforeRenderGrid: function () { }
    },
    settings = $.extend(defaults, option);
    var p = new Object();
    p.settings = settings;
    p.map = new Object();
    $.each(this, function (index) {
        var $obj = $(this);
        var fieldType = $obj.attr("fieldType");
        var fieldName = $obj.attr("name");
        var fieldlabel = $obj.attr("label");
        var fieldwidth = $obj.attr("fieldWidth");
        var fieldDisabled = $obj.attr("fieldDisabled") == "true";
        if (typeof fieldwidth == "string") {
            fieldwidth = parseFloat(fieldwidth);
        }
        if (fieldwidth == undefined || isNaN(parseFloat(fieldwidth)))
            fieldwidth = null;
        var fieldValue = null;
        if (p.settings.data != null && p.settings.data.length > 0) {
            fieldValue = p.settings.data[0][0][fieldName];
            fieldValue = fieldValue == undefined ? null : fieldValue;
        }
        var pObj = null;
        if (fieldType == "textbox") {// nullText: '不能为空'
            var metaML = null;
            if (p.settings.moduleConfig != null)
                metaML = p.settings.moduleConfig.metas[0][fieldName].MaxLength;
            pObj = $obj.ligerTextBox({
                labelWidth: p.settings.labelWidth,
                labelAlign: 'right',
                label: fieldlabel,
                width: fieldwidth,
                disabled: fieldDisabled,
                maxlength: metaML > 0 ? metaML : null
            });
            pObj.setValue(fieldValue);
        } else if (fieldType == "textarea") {// nullText: '不能为空'

            pObj = $obj
            if (fieldDisabled == "true")
                pObj.attr("readonly", "readonly");
            pObj.setValue = function (v) {
                $obj.val(v);
            }
            pObj.getValue = function () {
                return $obj.val();
            }
            pObj.setValue(fieldValue);
        }
        else if (fieldType == "date") {//, width: fieldwidth
            pObj = $obj.ligerDateEditor({
                format: "yyyy-MM-dd",
                labelWidth: p.settings.labelWidth,
                labelAlign: 'right',
                disabled: fieldDisabled,
                label: fieldlabel
            });
            pObj.setValue(fieldValue);
            pObj.getValue = function () {
                return $(pObj.element).val();
            }
        } else if (fieldType == "select") {
            $obj.val(fieldValue);
            pObj = $obj.ligerComboBox({ disabled: fieldDisabled });
            pObj.getValue = function () {
                return $obj.val();
            }
          
            pObj.setValue = function (val) {
                $obj.val(val);
                pObj.setSelect();
            }
        } else if (fieldType == "int") {
            //$("#txt1").ligerSpinner({ height: 28 });
            pObj = $obj.ligerSpinner({
                type: 'int',
                isNegative: false,
                minValue: 0,
                disabled: fieldDisabled,
                
            });
            pObj._setWidth(fieldwidth);
            //$("#txt3").ligerSpinner({ height: 28, type: 'int', isNegative: false });
            //$("#txt4").ligerSpinner({ height: 28, type: 'time' });
            pObj.setValue(fieldValue)
        } else if (fieldType == "float") {
            pObj = $obj.ligerSpinner({
                label: fieldlabel,
                labelWidth: p.settings.labelWidth,
                disabled: fieldDisabled,
                isNegative: false,
                labelAlign: 'right'
            });
            pObj.setValue(fieldValue)
        } else if (fieldType == "lookup") {

            var lookup = null
            if (p.settings.moduleConfig != null)
                lookup = p.settings.moduleConfig.lookups[fieldName];
            if (lookup != null && lookup.KeyField != null) {
                $obj.val(fieldValue);
                pObj = $obj.lookup({
                    disabled: fieldDisabled,
                    queryName: lookup.QueryName,
                    render: p,
                    textField: lookup.ReturnTextField,
                    valueFields: lookup.ReturnValueField.split(';'),
                    ParamFields: lookup.ParamFields.split(';'),
                    onSelected: function (data, cbObj) {
                    }
                });
            }

        } else if (fieldType == "grid") {
            //初始化表格
            var keyField = $obj.attr("keyField");
            var ObjIdx = $obj.attr("objIndex");
            var datarows = [];
            if (p.settings.data != null && p.settings.data[ObjIdx] != null) {
                datarows = p.settings.data[ObjIdx];
            }
            var lookup = null;
            var cols = null;
            fieldName = "grid" + ObjIdx;
            if (p.settings.moduleConfig != null) {
                lookup = p.settings.moduleConfig.lookups[keyField];
                cols = p.settings.moduleConfig.colsList[fieldName];
            }
            pObj = $obj.detailGrid({
                cols: cols,
                data: datarows,
                lookup: lookup,
                gridIndex: ObjIdx,
                onBeforeRender: p.settings.onBeforeRenderGrid
            });

        }
        if (pObj != null)
            p.map[fieldName] = { type: fieldType, obj: pObj }
    });
    //查找对象的方法
    p.find = function (fieldName) {
        return p.map[fieldName];
    }
    p.add = function (fieldName, fieldType, pObj) {
        p.map[fieldName] = { type: fieldType, obj: pObj }
    }
    p.serializeData = function (type) {
        var sData = new Object();
        var isGo = true;
        var msg = "";
        $(".error_border").removeClass("error_border");
        $.each(p.map, function (index) {
            var v = p.map[index].obj.getValue(type);
            var fMeta = p.settings.moduleConfig.metas[0][index];
            if (fMeta != null && fMeta.NotNull) {
                if (v == null || $.trim(v) == "") {
                    isGo = false;
                    p.setError(index);
                    msg += fMeta.Caption + " 不能为空！\r\n";
                }
            }
            sData[index] = v;
        });
        if (isGo)
            return sData;
        else {
            alert(msg);
            return false;
        }
    }
    p.setError = function (fieldname) {
        if (p.map[fieldname] != undefined)
            $(p.map[fieldname].obj.element).closest(".l-text").addClass("error_border");
    }
    return p;
}

//弹出选择框插件
$.fn.lookup = function (option) {
    var defaults = {
        disabled: false,
        queryName: '',
        url: '/Home/Lookup',
        multSelect: false,
        render: null,
        params: [],
        textField: "", //单选有效
        valueFields: [], //单选有效
        onSelected: function (data, cbObj) {

        }
    },
    settings = $.extend(defaults, option);
    var $obj = this;
    var p = new Object();
    p.element = $obj;
    p.settings = settings;
    p.setValue = function (val) {
        $obj.val(val);
    }
    //如果是单选状态，根据显示字段赋默认值
    pObj = $obj.ligerComboBox({
        onBeforeOpen: f_selectContact,
        disabled: p.settings.disabled
        /*valueFieldID: 'hidCustomerID', 
        width: 300*/
    });
    p.comboBox = pObj;
    p.getValue = function () {
        return p.element.val(); //p.comboBox.getValue();
    };
    function f_selectContact() {
        var paramsObj = new Object();
        if (p.settings.params != null && p.settings.params.length > 0) {
            $.each(p.settings.params, function (index) {
                paramsObj[p.settings.params[index]] = $("[name=" + p.settings.params[index] + "]").val();
            });

        }
        p.dialog = $.ligerDialog.open({
            title: '选择',
            name: 'winselector',
            width: 600,
            height: 400,
            isHidden: false,
            url: p.settings.url
                + '?queryName=' + p.settings.queryName
                + "&multSelect=" + p.settings.multSelect
                + "&pars=" + escape($.toJSON(paramsObj))
                + "&" + Math.random()
            ,
            buttons: [
                { text: '确定', onclick: f_selectContactOK },
                { text: '取消', onclick: f_selectContactCancel }
            ]
        });
        return false;
    }
    function f_selectContactOK(item, dialog) {

        //var fn = dialog.frame.f_select || dialog.frame.window.f_select;
        var dialogWindow = $("#winselector")[0].contentWindow;
        var fn = dialogWindow.f_select;
        var data = fn();
        if (!data) {
            alert('请选择行!');
            return;
        }
        if (!p.settings.multSelect) {
            $obj.val(data[p.settings.textField]);
            if (p.settings.valueFields != null && p.settings.valueFields.length > 0) {
                $.each(p.settings.valueFields, function (index) {
                    var fname = p.settings.valueFields[index];
                    $("[name=" + fname + "]").val(data[fname]);
                    if (p.settings.render && p.settings.render.find(fname)) {
                        p.settings.render.find(fname).obj.setValue(data[fname]);
                    }
                });
            }
        }
        settings.onSelected.call(p, data, pObj);
        dialog.close();
    }
    function f_selectContactCancel(item, dialog) {
        dialog.close();
    }
    return p;
}
//详细表格插件
$.fn.detailGrid = function (option) {
    var defaults = {
        gridIndex: 1,
        cols: [],
        lookup: null,
        data: [],
        isRepeat: false,
        url: '/Home/Lookup',
        serverTotal: 'Other2',
        onSelected: function (data) {
            if (data.length > 0) {
                var griddata = p.grid.getData();
                var newdata = new Array();
                var keyfield = p.settings.lookup.KeyField;
                var isUpdate = false;
                if (data.length > 0) {
                    $.each(data, function () {
                        if (!checkData(griddata, keyfield, this[keyfield])) {
                            newdata.push(this);
                            p.grid.addRow(this);
                            isUpdate = true;
                        }
                    });
                    griddata.Data = newdata;
                    griddata.Other1 = newdata.length;
                }
                //                if (isUpdate)
                //                    p.grid.loadData(griddata);
            }
        },
        onDelete: function (data) {
            var rows = p.grid.getCheckedRows();
            if (rows == null || rows.length == 0) { alert('请选择行'); return; }
            $.each(rows, function () {
                p.grid.deleteRow(this);
            });

        },
        onBeforeRender: function () {

        }
    },
   settings = $.extend(defaults, option);
    var $obj = this;
    var p = new Object();
    p.element = $obj;
    p.settings = settings;

    var gridData = { Data: p.settings.data, Other1: p.settings.data.length };

    var hasPhoto = false;
    $.each(p.settings.cols, function (i) {
        if (this.type == 'photo') {
            hasPhoto = true;
            return false;
        }
    });
    p.settings.barItems = [
                { text: '增加', click: add_itemclick, icon: 'add' },
                { line: true },
                { text: '删除', click: del_itemclick, icon: 'delete' }
    ];
    p.currentEditRow = null;
    p.settings.onDblClickRow = function (rowdata, rowindex, rowDomElement) {
        //if (p.currentEditRow != null && this.getRow(p.currentEditRow)!=null)
        //    this.endEdit(p.currentEditRow);
        //var row = this.getSelectedRow();
        //p.currentEditRow = row;
        if (!rowdata) { alert('请选择行'); return; }
        this.beginEdit(rowdata);
    }
    p.settings.onBeforeRender.call(p);
    p.grid = $obj.ligerGrid({
        statusName: "UpdateFlag" + p.settings.gridIndex,
        columns: p.settings.cols,
        method: 'get',
        delayLoad: false,
        dataType: "local",
        dataAction: 'local',
        root: 'Data',                       //数据源字段名
        record: 'Other1',                     //数据源记录数字段名
        rownumbers: true,
        data: gridData,
        showTitle: false,
        checkbox: true,
        width: '99.5%',
        height: '100%',
        //height: '500px',
        heightDiff: -45,
        rowHeight: hasPhoto ? 50 : undefined,
        enabledSort: false,
        clickToEdit: false, enabledEdit: true,
        onDblClickRow: p.settings.onDblClickRow,
        onAfterEdit: p.settings.onAfterEdit,
        toolbar: { items: p.settings.barItems }
    });



    function add_itemclick() {
        if (p.settings.lookup == null || p.settings.lookup.KeyField == null) {
            alert("并没有设置专业选择");
            return false;
        }
        var paramsObj = new Object();
        var params = p.settings.lookup.ParamFields.split(';')
        if (params != null && params.length > 0) {
            $.each(params, function (index) {
                paramsObj[params[index]] = $("[name=" + params[index] + "]").val();
            });

        }
        p.dialog = $.ligerDialog.open({
            title: '选择',
            name: 'winselector',
            width: 600,
            height: 400,
            isHidden: false,
            url: p.settings.url
                    + '?queryName=' + p.settings.lookup.QueryName
                    + "&multSelect=" + true
                    + "&SelectAll=" + p.settings.lookup.IsSelectAll
                    + "&pars=" + escape($.toJSON(paramsObj))
                    + "&" + Math.random(),
            buttons: [
                    { text: '确定', onclick: f_selectContactOK },
                    { text: '取消', onclick: f_selectContactCancel }
            ]

        });
    }
    function del_itemclick() {
        p.settings.onDelete();
    }

    //获取数据的方法
    p.getValue = function (type) {
        p.grid.endEdit();
        var data = p.grid.getData();
        var newdata = new Array();
        var deleteData = p.grid.getDeleted();
        if (deleteData != null && deleteData.length > 0)
            $.each(deleteData, function (j) {
                newdata.push(deleteData[j]);
            });
        $.each(data, function (i) {
            if (data[i][p.grid.statusName] != 'nochanged') {
                newdata.push(this);
            }
        });

        return newdata;
    }
    function f_selectContactOK(item, dialog) {

        //var fn = dialog.frame.f_select || dialog.frame.window.f_select;
        var dialogWindow = $("#winselector")[0].contentWindow;
        var fn = dialogWindow.f_select;
        var data = fn();
        if (!data) {
            alert('请选择行!');
            return;
        }
        p.settings.onSelected.call(p, data);
        dialog.close();
    }
    function f_selectContactCancel(item, dialog) {
        dialog.close();
    }
    //通过字段名和值，判断数据中是否存在
    function checkData(data, keyfield, value) {
        var rs = false;
        if (data.length > 0) {
            $.each(data, function (index) {
                if (this[keyfield] == value) {
                    rs = true;
                    return false;
                }
            });
        }
        return rs;
    }
    return p;
}



