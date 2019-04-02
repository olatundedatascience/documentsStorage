const fs = require('fs');
const path = require('path');

var lib ={}

lib.baseDr = path.join(__dirname, '/../.data/');

lib.read = function(filename, dir, callback) {
    
        fs.readFile(lib.baseDr+dir+'/'+filename+'.json','utf8',(er, data)=> {
        
            callback(null, data)
        
    })
        
   
   
    
}

lib.delete = function(filen, dir, callback) {
    fs.unlink(lib.baseDr+dir+'/'+filen.json, (er)=> {
        if(!er) {
            callback(false)
        }
        else {
            callback('error deleting file')
        }
    })
}

lib.update= function(dir, filen, data, callback) {
    fs.open(lib.baseDr+dir+'/'+filen+'.json', 'r+', (er, fd)=> {
        if(!er && fd) {
            var dataString = JSON.stringify(data);

           /* fs.truncate(fd, (er)=> {
                if(!er) {
*/
                    fs.writeFile(fd, dataString, (er)=> {
                        if(!er) {
                            fs.close(fd, (er)=> {
                                if(!er) {
                                    callback(false)
                                }
                                else {
                                    callback('done but error in closing file')
                                }
                            })
                        }
                        else {
                            callback("error updating file")
                        }
                    })

                }
                else {
                    callback('error truncating file')
                }
            //})
/*
        }

        else {
            callback("Could not open the specified file")
        }
        */
    })
}

lib.create = function(file, dir, data, callback) {
    fs.open(lib.baseDr+dir+'/'+file+'.json', 'wx', (er, fd)=> {
        if(!er && fd) {
            var stringData = JSON.stringify(data);
            fs.writeFile(fd, stringData, (er)=> {
                if(!er) {
                    fs.close(fd, (er)=> {
                        if(!er) {
                            callback(false)
                        }
                        else {
                            callback(er)
                        }
                    })
                }
                else {
                    callback(er)
                }
            } )

        }
        else {
            callback(er)
        }
    })
}

module.exports = lib;