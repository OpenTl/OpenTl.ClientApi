
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"ISocks Proxy",
        content:"ISocks Proxy",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"SessionWriter",
        content:"SessionWriter",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"MemorySessionStore",
        content:"MemorySessionStore",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"CloudPasswordNeededException",
        content:"CloudPasswordNeededException",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"IClientSession",
        content:"IClientSession",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"Socks ProxyConfig",
        content:"Socks ProxyConfig",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"IClientSettings",
        content:"IClientSettings",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"IFactorySettings",
        content:"IFactorySettings",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"IRequestSender",
        content:"IRequestSender",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"IUpdatesRaiser",
        content:"IUpdatesRaiser",
        description:'',
        tags:''
    });

    a({
        id:10,
        title:"CustomRequestsService",
        content:"CustomRequestsService",
        description:'',
        tags:''
    });

    a({
        id:11,
        title:"FactorySettings",
        content:"FactorySettings",
        description:'',
        tags:''
    });

    a({
        id:12,
        title:"UserNotAuthorizeException",
        content:"UserNotAuthorizeException",
        description:'',
        tags:''
    });

    a({
        id:13,
        title:"ESystemNotification",
        content:"ESystemNotification",
        description:'',
        tags:''
    });

    a({
        id:14,
        title:"IHelpService",
        content:"IHelpService",
        description:'',
        tags:''
    });

    a({
        id:15,
        title:"SessionExtensions",
        content:"SessionExtensions",
        description:'',
        tags:''
    });

    a({
        id:16,
        title:"UnhandledException",
        content:"UnhandledException",
        description:'',
        tags:''
    });

    a({
        id:17,
        title:"UsersService",
        content:"UsersService",
        description:'',
        tags:''
    });

    a({
        id:18,
        title:"IUpdatesService",
        content:"IUpdatesService",
        description:'',
        tags:''
    });

    a({
        id:19,
        title:"UserLogoutException",
        content:"UserLogoutException",
        description:'',
        tags:''
    });

    a({
        id:20,
        title:"ITemploraryClientCache",
        content:"ITemploraryClientCache",
        description:'',
        tags:''
    });

    a({
        id:21,
        title:"IFileService",
        content:"IFileService",
        description:'',
        tags:''
    });

    a({
        id:22,
        title:"ClientFactory",
        content:"ClientFactory",
        description:'',
        tags:''
    });

    a({
        id:23,
        title:"IUsersService",
        content:"IUsersService",
        description:'',
        tags:''
    });

    a({
        id:24,
        title:"IMessagesService",
        content:"IMessagesService",
        description:'',
        tags:''
    });

    a({
        id:25,
        title:"IAuthService",
        content:"IAuthService",
        description:'',
        tags:''
    });

    a({
        id:26,
        title:"IContactsService",
        content:"IContactsService",
        description:'',
        tags:''
    });

    a({
        id:27,
        title:"IApplicationProperties",
        content:"IApplicationProperties",
        description:'',
        tags:''
    });

    a({
        id:28,
        title:"ISessionStore",
        content:"ISessionStore",
        description:'',
        tags:''
    });

    a({
        id:29,
        title:"IClientApi",
        content:"IClientApi",
        description:'',
        tags:''
    });

    a({
        id:30,
        title:"INettyBootstrapper",
        content:"INettyBootstrapper",
        description:'',
        tags:''
    });

    a({
        id:31,
        title:"FileMigrationException",
        content:"FileMigrationException",
        description:'',
        tags:''
    });

    a({
        id:32,
        title:"IContextGetter",
        content:"IContextGetter",
        description:'',
        tags:''
    });

    a({
        id:33,
        title:"ISessionWriter",
        content:"ISessionWriter",
        description:'',
        tags:''
    });

    a({
        id:34,
        title:"PhoneCodeInvalidException",
        content:"PhoneCodeInvalidException",
        description:'',
        tags:''
    });

    a({
        id:35,
        title:"FloodWaitException",
        content:"FloodWaitException",
        description:'',
        tags:''
    });

    a({
        id:36,
        title:"ILogoutService",
        content:"ILogoutService",
        description:'',
        tags:''
    });

    a({
        id:37,
        title:"ApplicationProperties",
        content:"ApplicationProperties",
        description:'',
        tags:''
    });

    a({
        id:38,
        title:"UpdateHandler",
        content:"UpdateHandler",
        description:'',
        tags:''
    });

    a({
        id:39,
        title:"ICustomRequestsService",
        content:"ICustomRequestsService",
        description:'',
        tags:''
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/ISocks5Proxy',
        title:"ISocks5Proxy",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Settings/SessionWriter',
        title:"SessionWriter",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Settings/MemorySessionStore',
        title:"MemorySessionStore",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/CloudPasswordNeededException',
        title:"CloudPasswordNeededException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/IClientSession',
        title:"IClientSession",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/Socks5ProxyConfig',
        title:"Socks5ProxyConfig",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/IClientSettings',
        title:"IClientSettings",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/IFactorySettings',
        title:"IFactorySettings",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/IRequestSender',
        title:"IRequestSender",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/IUpdatesRaiser',
        title:"IUpdatesRaiser",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services/CustomRequestsService',
        title:"CustomRequestsService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/FactorySettings',
        title:"FactorySettings",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/UserNotAuthorizeException',
        title:"UserNotAuthorizeException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Enums/ESystemNotification',
        title:"ESystemNotification",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IHelpService',
        title:"IHelpService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Extensions/SessionExtensions',
        title:"SessionExtensions",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/UnhandledException',
        title:"UnhandledException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services/UsersService',
        title:"UsersService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IUpdatesService',
        title:"IUpdatesService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/UserLogoutException',
        title:"UserLogoutException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Client.Interfaces/ITemploraryClientCache',
        title:"ITemploraryClientCache",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IFileService',
        title:"IFileService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/ClientFactory',
        title:"ClientFactory",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IUsersService',
        title:"IUsersService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IMessagesService',
        title:"IMessagesService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IAuthService',
        title:"IAuthService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/IContactsService',
        title:"IContactsService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/IApplicationProperties',
        title:"IApplicationProperties",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/ISessionStore',
        title:"ISessionStore",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/IClientApi',
        title:"IClientApi",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/INettyBootstrapper',
        title:"INettyBootstrapper",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/FileMigrationException',
        title:"FileMigrationException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Interfaces/IContextGetter',
        title:"IContextGetter",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/ISessionWriter',
        title:"ISessionWriter",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/PhoneCodeInvalidException',
        title:"PhoneCodeInvalidException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto.Exceptions/FloodWaitException',
        title:"FloodWaitException",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.MtProto/ILogoutService',
        title:"ILogoutService",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi/ApplicationProperties',
        title:"ApplicationProperties",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/UpdateHandler',
        title:"UpdateHandler",
        description:""
    });

    y({
        url:'/OpenTl.ClientApi/api/OpenTl.ClientApi.Services.Interfaces/ICustomRequestsService',
        title:"ICustomRequestsService",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
