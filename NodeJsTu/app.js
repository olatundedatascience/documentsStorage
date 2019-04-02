const port = 4300;
const http = require('http');
const fs = require('fs');
const path = require('path');
const url = require('url');
const StringDecoder = require('string_decoder').StringDecoder;
const router = require('./route');
const _data = require('./lib/data');
http.createServer(function(req, res){
    var method = req.method.toLowerCase();
    var headers = req.headers;
    var stringd = new StringDecoder('utf-8');
    var buffer = "";
    var parsedUrl = url.parse(req.url, true);
    var pathname = parsedUrl.pathname;
    var trimmedPath = pathname.replace(/^\/+|\/+$/g, '');

    var chosenHandler = typeof(router[trimmedPath]) != 'undefined' ? router[trimmedPath] : router['notFound'];

    
    req.on('data', (data)=> {
        buffer += stringd.write(data);
    });

    req.on('end', ()=> {
        buffer += stringd.end();
        var data = {
            'trimmedpath': trimmedPath,
            'headers' : headers,
            'method' : method
        }



        chosenHandler(data, (statusCode, payload)=>{
                statusCode = typeof(statusCode) == 'nuumber' ? statusCode : 200;
                payload = typeof(payload) == 'object' ? payload : {};

                let payloadString = JSON.stringify(payload);
               /* 
                _data.create('info', 'test',data, (er)=> {
                        console.log(er);
                })
                */
                

                _data.update('test', 'info', payload, (er)=> {
                    console.log(er);
                })
                
               res.writeHead(statusCode);
               _data.read('info', 'test', (err, data)=> {
                  // console.log(data)
                  res.end(data)
               })
                
               // res.end(payloadString);
        })

        //  res.end(buffer)  ;
        //console.log(`The following payload received ${buffer}`);
    })

}).listen(port, ()=> {
    console.log(`Server listening on port ${port}`);
})

/*
var handlers ={}
handlers.sample = function(data, callback) {
    callback(406, {'name': 'abiodun'})
}
handlers.notFound = (data,callback) => {
    callback(404)
}
var router ={
    'sample': handlers.sample,
    //'notFound': handlers.notFound
}
*/