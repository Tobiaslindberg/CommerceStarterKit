function epiGaect(settings) {
    this._settings = settings;

    this._trans = function(index, value){
        if (value){
            this._settings.trans[index] = value.toString();
        }
        return this._settings[index];
    };

    this.store = function (value) {
        return this._trans(2, value);
    };
    this.total = function (value) {
        return this._trans(3, value);
    };
    this.tax = function (value) {
        return this._trans(4, value);
    };
    this.shipping = function (value) {
        return this._trans(5, value);
    };
    this.city = function (value) {
        return this._trans(6, value);
    };
    this.state = function (value) {
        return this._trans(7, value);
    };
    this.country = function (value) {
        return this._trans(8, value);
    };

    this.addItem = function (item) {
        this._settings.items.push(item);
    };
    this.removeItem = function (index, numberToRemove) {
        this._settings.items = this._settings.items.splice(index, numberToRemove || 1);
    };

    this.track = function () {
        if (this._settings.trans && this._settings.items) {
            _gaq.push(this._settings.trans);
            for (var i in this._settings.items) {
                _gaq.push(this._settings.items[i]);
            }
            _gaq.push(['_trackTrans']);        }
    };
};
