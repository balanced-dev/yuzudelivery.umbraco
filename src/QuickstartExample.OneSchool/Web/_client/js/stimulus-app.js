const StimulusApp = Stimulus.Application.start();

StimulusApp.settings = {
    isFrontend: document.documentElement.getAttribute('data-frontend-environment') === 'true',
}

StimulusApp.utilites = {
    simulateNetworkDelay: async function(callback = function(){}, minStallTime = 200, maxStallTime = 5000) {
        if(StimulusApp.settings.isFrontend) {
            const stallTime = Math.random() * (maxStallTime - minStallTime) + minStallTime;
            await new Promise(resolve => setTimeout(resolve, stallTime));
            callback();
        }
        else {
            callback();
        }
    },    
    
    // https://davidwalsh.name/javascript-debounce-function
    debounce: function(func, wait, immediate) {
        var timeout;
        return function() {
            var context = this, args = arguments;
            var later = function() {
                timeout = null;
                if (!immediate) func.apply(context, args);
            };
            var callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func.apply(context, args);
        };
    },

    // http://sampsonblog.com/simple-throttle-function/
    throttle: function(callback, limit) {
        var wait = false;                 
        return function () {              
            if (!wait) {
                callback.call();
                wait = true;
                setTimeout(function () {
                    wait = false;
                }, limit);
            }
        }
    },

    formSerialise(formElement) {
        return Array.from(
            new FormData(formElement),
            e => e.map(encodeURIComponent).join('=')
        ).join('&');
    },
}