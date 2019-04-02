var handlers ={}
handlers.sample = function(data, callback) {
    callback(406, {'name': 'abiodun'})
}
handlers.notFound = (data,callback) => {
    callback(404)
}
var router ={
    'sample': handlers.sample,
    'notFound': handlers.notFound
}

module.exports = router;