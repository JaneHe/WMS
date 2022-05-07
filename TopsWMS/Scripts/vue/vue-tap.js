/**
 * Created by ���� on 15/12/6.
 */
/*
 * ��������ָ��
 * v-tap=handler
 * or
 * ������ָ��
 * v-tap=handler($index,el,$event)
 *
 * !!!����!!!
 * ��tapObj����ע����ԭ��event������
 * 	event.tapObjӵ��6��ֵ
 * 	pageX,pageY,clientX,clientY,distanceX,distanceY
 * ����2���ֱ����ָ�����ƶ���λ��(�Ժ��������չ����)
 *
 * */
;(function () {
  var vueTap = {};
  var isVue2 = false;

  /**                               ���÷�����ʼ                 **/
  function isPc() {
    var uaInfo = navigator.userAgent;
    var agents = ["Android", "iPhone", "Windows Phone", "iPad", "iPod"];
    var flag = true;
    for ( var i = 0; i < agents.length; i++ ) {
      if (uaInfo.indexOf(agents[i]) > 0) {
        flag = false;
        break;
      }
    }
    return flag;
  }

  function isTap(self) {
    if (isVue2 ? self.disabled : self.el.disabled) {
      return false;
    }
    var tapObj = self.tapObj;
    return self.time < 300 && Math.abs(tapObj.distanceX) < 10 && Math.abs(tapObj.distanceY) < 10;
  }

  function touchstart(e, self) {
    var touches = e.touches[0];
    var tapObj = self.tapObj;
    tapObj.pageX = touches.pageX;
    tapObj.pageY = touches.pageY;
    tapObj.clientX = touches.clientX;
    tapObj.clientY = touches.clientY;
    self.time = +new Date();
  }

  function touchend(e, self) {
    var touches = e.changedTouches[0];
    var tapObj = self.tapObj;
    self.time = +new Date() - self.time;
    tapObj.distanceX = tapObj.pageX - touches.pageX;
    tapObj.distanceY = tapObj.pageY - touches.pageY;

    if (!isTap(self)) return;
    self.handler(e);

  }

  /**                               ���÷�������                 * */

  var vue1 = {
    isFn: true,
    acceptStatement: true,
    update: function (fn) {
      var self = this;
      self.tapObj = {};
      if (typeof fn !== 'function' && self.el.tagName.toLocaleLowerCase() !== 'a') {
        return console.error('The param of directive "v-tap" must be a function!');
      }

      self.handler = function (e) { //This directive.handler
        e.tapObj = self.tapObj;
        if (self.el.href && !self.modifiers.prevent) {
          return window.location = self.el.href;
        }

        var tagName = e.target.tagName.toLocaleLowerCase();
        if(tagName === 'input' || tagName === 'textarea') {
          return e.target.focus();
        }

        fn.call(self, e);
      };
      if (isPc()) {
        self.el.addEventListener('click', function (e) {
          if (self.el.href && !self.modifiers.prevent) {
            return window.location = self.el.href;
          }
          self.handler.call(self, e);
        }, false);
      } else {
        self.el.addEventListener('touchstart', function (e) {

          if (self.modifiers.stop)
            e.stopPropagation();
          if (self.modifiers.prevent)
            e.preventDefault();
          touchstart(e, self);
        }, false);
        self.el.addEventListener('touchend', function (e) {
          try {
            Object.defineProperty(e, 'currentTarget', {// ��дcurrentTarget���� ��jq��ͬ
              value: self.el,
              writable: true,
              enumerable: true,
              configurable: true
            })
          } catch (e) {
            // ios 7�¶� e.currentTarget ��defineProperty�ᱨ����
            // ����TypeError��Attempting to configurable attribute of unconfigurable property������
            // ��catch����д
            console.error(e.message)
            e.currentTarget = self.el
          }
          e.preventDefault();

          return touchend(e, self);
        }, false);
      }
    }
  };

  var vue2 = {
    bind: function (el, binding) {
      el.tapObj = {};
      el.handler = function (e,isPc) { //This directive.handler
        var value = binding.value;

        if (!value && el.href && !binding.modifiers.prevent) {
          return window.location = el.href;
        }

        value.event = e;
        var tagName = value.event.target.tagName.toLocaleLowerCase();
        !isPc ? value.tapObj = el.tapObj : null;

        if(tagName === 'input' || tagName === 'textarea') {
          return value.event.target.focus();
        }

        value.methods.call(this, value);
      };
      if (isPc()) {
        el.addEventListener('click', function (e) {

          if (binding.modifiers.stop)
            e.stopPropagation();
          if (binding.modifiers.prevent)
            e.preventDefault();
          el.handler(e, true)
        }, false);
      } else {
        el.addEventListener('touchstart', function (e) {

          if (binding.modifiers.stop)
            e.stopPropagation();
          if (binding.modifiers.prevent)
            e.preventDefault();
          touchstart(e, el);
        }, false);
        el.addEventListener('touchend', function (e) {
          try {
            Object.defineProperty(e, 'currentTarget', {// ��дcurrentTarget���� ��jq��ͬ
              value: el,
              writable: true,
              enumerable: true,
              configurable: true
            })
          } catch (e) {
            // ios 7�¶� e.currentTarget ��defineProperty�ᱨ����
            // ����TypeError��Attempting to configurable attribute of unconfigurable property������
            // ��catch����д
            console.error(e.message)
            e.currentTarget = el
          }
          e.preventDefault();

          return touchend(e, el);
        }, false);
      }
    },
    componentUpdated : function(el,binding) {
      el.tapObj = {};
      el.handler = function (e,isPc) { //This directive.handler
        var value = binding.value;
        if (!value && el.href && !binding.modifiers.prevent) {
          return window.location = el.href;
        }
        value.event = e;
        !isPc ? value.tapObj = el.tapObj : null;
        value.methods.call(this, value);
      };
    },
    unbind: function (el) {
      // ж�أ���˵�˶�����
      el.handler = function () {};
    }
  };

  vueTap.install = function (Vue) {
    if (Vue.version.substr(0, 1) > 1) {
      isVue2 = true;
    }

    Vue.directive('tap', isVue2 ? vue2 : vue1);
  };
  vueTap.version = '3.0.3';

  if (typeof exports == "object") {
    module.exports = vueTap;
  } else if (typeof define == "function" && define.amd) {
    define([], function () {
      return vueTap
    })
  } else if (window.Vue) {
    window.vueTap = vueTap;
    Vue.use(vueTap);
  }

})();