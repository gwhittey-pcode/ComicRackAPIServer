(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-a924b2c4"],{"2e90":function(t,s,e){"use strict";e.r(s);var i=function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("v-layout",{attrs:{"justify-center":"",column:"",scroll:""}},[e("v-data-iterator",{staticClass:"elevation-1 pages",attrs:{items:this.results,"rows-per-page-items":t.rowsPerPageItems,pagination:t.pagination,"content-tag":"v-layout",row:"",wrap:"","justify-space-around":"",scroll:"",width:"100%","hide-actions":""},on:{"update:pagination":function(s){t.pagination=s}},scopedSlots:t._u([{key:"item",fn:function(s){return e("div",{staticClass:"cellcontainer",on:{click:t.selectItem}},[e("div",{staticClass:"thumb"},[e("div",[e("img",{attrs:{id:s.item.Id,src:t.baseURL+"/API/Comics/"+s.item.Id+"/Pages/0?height=240","lazy-src":t.baseURL+"/API/Comics/"+s.item.Id+"/Pages/0?height=240"}})])]),e("div",{staticClass:"label"},[t._v(t._s(s.item.Title)+" ("+t._s(s.item.Volume)+")")])])}}])}),e("div",{staticClass:"text-xs-center pt-2"},[e("v-pagination",{attrs:{length:t.pages},model:{value:t.pagination.page,callback:function(s){t.$set(t.pagination,"page",s)},expression:"pagination.page"}})],1)],1)},a=[],n=e("1157"),r=e.n(n),o=e("1a72"),c=e("2b0e"),u=e("dc96"),l=e.n(u),h=r.a;c["default"].use(l.a);var p={created:function(){h(".back-btn").addClass("hidden"),this.$store.dispatch(o["i"]),this.results=this.$store.state.api.comicList},methods:{selectItem:function(t){this.$store.dispatch(o["j"],t.target.id),this.$router.push("/seriesissues")}},computed:{search:function(){return this.$store.state.api.search},pages:function(){return null==this.pagination.rowsPerPage||null==this.pagination.totalItems?0:Math.ceil(this.pagination.totalItems/this.pagination.rowsPerPage)},Issue:function(){return this.$store.state.api.selectedIssue[0]},baseURL:function(){return this.$store.state.api.baseURL},SeriesList:function(){return this.$store.state.api.comicList}},watch:{search:function(){var t=this;""===this.search.trim()?this.results=this.SeriesList:this.$search(this.search,this.SeriesList,{keys:["Title"]}).then(function(s){t.results=s})},results:function(){var t=this;this.$nextTick(function(){t.pagination.totalItems=t.results.length})}},mounted:function(){this.$store.state.test=this.$el.offsetHeight},data:function(){return{rowsPerPageItems:[5,10,15],pagination:{rowsPerPage:this.$store.state.api.gridSize},listtype:"Series",dialog:!1,results:this.SeriesList}}},g=p,d=(e("5922"),e("2877")),f=Object(d["a"])(g,i,a,!1,null,"7f2c19cc",null);s["default"]=f.exports},5922:function(t,s,e){"use strict";var i=e("6988"),a=e.n(i);a.a},6988:function(t,s,e){}}]);
//# sourceMappingURL=chunk-a924b2c4.b42409cc.js.map