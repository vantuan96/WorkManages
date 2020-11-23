//Prefix url
var _prefixAdminDomain = '/Admin';

//model for modal

class AJAXModel_Modal {
    constructor(url, idrecord, idmodal, idboxrender, isupdate, title, idsub) {
        this.url = url;
        this.idrecord = idrecord;
        this.idmodal = idmodal;
        this.idboxrender = idboxrender;
        this.isupdate = isupdate;
        this.title = title;
        this.idsub = idsub;
    }
}