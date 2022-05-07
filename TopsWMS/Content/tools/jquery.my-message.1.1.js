/**
 * 鏀寔娴忚鍣�:IE8+
 * */
var MyMessage = (function() {
	function message(setting) {
		//鍚堝苟榛樿鍙傛暟
		this.messageContainer = null;
		this.opts = null;
		this._setting(setting);
		this.init();
	}

	/*榛樿鍙傛暟*/
	message.DEFAULTS = {
		iconFontSize: "20px", //鍥炬爣澶у皬
		messageFontSize: "12px", //淇℃伅瀛椾綋澶у皬
		showTime: 3000, //娑堝け鏃堕棿
		align: "center", //鏄剧ず鐨勪綅缃�
		positions: { //鏀剧疆淇℃伅鐨勮寖鍥�
			top: "10px",
			bottom: "10px",
			right: "10px",
			left: "10px"
		},
		message: "杩欐槸涓€鏉℃秷鎭�", //娑堟伅鍐呭
		type: "normal", //娑堟伅鐨勭被鍨嬶紝杩樻湁success,error,warning绛�
	}
	/*璁剧疆娑堟伅鐨勫弬鏁�*/
	message.prototype._setting = function(setting) {
		this.opts = $.extend({}, message.DEFAULTS, setting);
	}
	message.prototype.setting = function(name, val) {
		if("object" === typeof name) {
			for(var k in name) {
				this.opts[k] = name[k]
			}
		} else if("string" === typeof name) {
			this.opts[name] = val;
		}
	}
	/*
	 鐢ㄤ簬娣诲姞鐩稿簲鐨刣om鍒癰ody鏍囩涓�
	 * */
	message.prototype.init = function() {
		var domStr = "<div class='m-message' style='top:" +
			this.opts.positions.top +
			";right:" +
			this.opts.positions.right +
			";left:" +
			this.opts.positions.left +
			";width:calc(100%-" +
			this.opts.positions.right +
			this.opts.positions.left +
			");bottom:" + this.opts.positions.bottom +
			"'></div>"
		this.messageContainer = $(domStr);
		this.messageContainer.appendTo($('body'))
	}
	/*
	 鐢ㄤ簬娣诲姞娑堟伅鍒扮浉搴旂殑鏍囩涓�
	 message:鍏蜂綋鐨勬秷鎭�
	 type:娑堟伅鐨勭被鍨�
	 * */
	message.prototype.add = function(message, type) {
		var domStr = "";
		type = type || this.opts.type;
		domStr += "<div class='c-message-notice' style='" +
			"text-align:" +
			this.opts.align +
			";'><div class='m_content'><i class='";
		switch(type) {
			case "normal":
				domStr += "icon-bubble";
				break;
			case "success":
				domStr += "icon-check-alt";
				break;
			case "error":
				domStr += "icon-notification";
				break;
			case "warning":
				domStr += "icon-cancel-circle";
				break;
			default:
				throw "浼犻€掔殑鍙傛暟type閿欒锛岃浼犻€抧ormal/success/error/warning涓殑涓€绉�";
				break;
		}
		domStr += "' style='font-size:" +
			this.opts.iconFontSize +
			";'></i><span style='font-size:" +
			this.opts.messageFontSize +
			";'>" + message + "</span></div></div>";
		var $domStr = $(domStr).appendTo(this.messageContainer);
		this._hide($domStr);
	}
	/**
	 * 闅愯棌娑堟伅
	 * $domStr锛氳娑堟伅鐨刯q瀵硅薄
	 * */
	message.prototype._hide = function($domStr) {
		setTimeout(function() {
			$domStr.fadeOut(1000);
		}, this.opts.showTime);
	}
	return {
		message: message /*瀵瑰鎻愪緵鐨勬帴鍙�*/
	}
})();