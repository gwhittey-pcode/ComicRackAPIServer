(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-6cf6f9b7"],{"7c47":function(t,s,i){"use strict";i.r(s);var e=function(){var t=this,s=t.$createElement,i=t._self._c||s;return i("v-layout",{attrs:{"justify-center":"",column:""}},[i("v-data-iterator",{staticClass:"elevation-1 gh",attrs:{items:t.issuesList,"rows-per-page-items":t.rowsPerPageItems,pagination:t.pagination,"content-tag":"v-layout",row:"",wrap:"","justify-space-around":"",width:"100%","hide-actions":""},on:{"update:pagination":function(s){t.pagination=s}},scopedSlots:t._u([{key:"item",fn:function(s){return i("div",{staticClass:"cellcontainer",on:{click:t.selectItem}},[i("div",{staticClass:"thumb"},[i("div",[i("img",{attrs:{id:s.item.Id,src:t.baseURL+"/API/Comics/"+s.item.Id+"/Pages/0?height=240","lazy-src":t.baseURL+"/API/Comics/"+s.item.Id+"/Pages/0?height=240"}}),i("v-progress-linear",{staticClass:"progBar",attrs:{color:"success",height:"3",value:Math.floor(s.item.CurrentPage/s.item.PageCount*100)}})],1),i("div",{staticClass:"label"},[t._v("#"+t._s(s.item.Number)+" "+t._s(s.item.Title)+" ("+t._s(s.item.Volume)+")")])])])}}])}),i("div",{staticClass:"text-xs-center pt-2 gf"},[i("v-pagination",{attrs:{length:t.pages},model:{value:t.pagination.page,callback:function(s){t.$set(t.pagination,"page",s)},expression:"pagination.page"}})],1),i("v-dialog",{attrs:{fullscreen:"","hide-overlay":"",transition:"dialog-bottom-transition"},model:{value:t.dialog,callback:function(s){t.dialog=s},expression:"dialog"}},[i("v-card",[i("v-toolbar",{attrs:{dark:"",color:"primary"}},[i("v-btn",{attrs:{icon:"",dark:""},nativeOn:{click:function(s){t.dialog=!1}}},[i("v-icon",[t._v("close")])],1),i("v-toolbar-title",[t._v(t._s(t.Issue.Series)+" ("+t._s(t.Issue.Volume)+")")]),i("v-spacer"),i("v-toolbar-items",[i("v-btn",{attrs:{dark:"",flat:""},nativeOn:{click:function(s){t.dialog=!1,t.readIssue()}}},[t._v("Read")])],1)],1),i("v-list",{attrs:{"three-line":"",subheader:""}},[i("div",{staticClass:"cover thumb"},[i("div",[i("img",{attrs:{src:this.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/0?height=240",onerror:"this.src='./publishers/blank.jpg'"}})])]),i("v-subheader",[t._v("User Controls")]),i("v-list-tile",{attrs:{avatar:""}},[i("v-list-tile-content",[i("v-list-tile-title",[t._v("Issue")]),i("v-list-tile-sub-title",[t._v(t._s(t.Issue.Caption))])],1)],1),i("v-list-tile",{attrs:{avatar:""}},[i("v-list-tile-content",[i("v-list-tile-title",[t._v("Summary")])],1)],1),i("div",{staticClass:"summary"},[t._v(t._s(t.Issue.Summary))]),i("v-list-tile",{attrs:{avatar:""}},[i("v-list-tile-content",[i("v-list-tile-title",[t._v("Publisher")])],1)],1),i("img",{staticClass:"publisherTbn",attrs:{src:"./publishers/"+t.Issue.Publisher+".jpg"}})],1),i("v-divider"),i("div",{staticClass:"preview"},[i("div",{staticClass:"thumb"},[i("div",[i("v-img",{attrs:{transition:"",contain:"",src:t.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/1?height=240"}})],1)]),i("div",{staticClass:"thumb"},[i("div",[i("v-img",{attrs:{transition:"",contain:"",src:t.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/2?height=240"}})],1)]),i("div",{staticClass:"thumb"},[i("div",[i("v-img",{attrs:{transition:"",contain:"",src:this.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/3?height=240"}})],1)]),i("div",{staticClass:"thumb"},[i("div",[i("v-img",{attrs:{transition:"",contain:"",src:t.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/4?height=240"}})],1)]),i("div",{staticClass:"thumb"},[i("div",[i("v-img",{attrs:{transition:"",contain:"",src:t.baseURL+"/API/Comics/"+t.Issue.Id+"/Pages/5?height=240"}})],1)])])],1)],1)],1)},a=[],n=i("1157"),r=i.n(n),o=i("1a72"),l=r.a,c={created:function(){l(".back-btn").removeClass("hidden"),this.issuesList[0].Series,this.$store.dispatch(o["e"],this.issuesList[0].Series)},methods:{selectItem:function(t){this.$store.dispatch(o["e"],t.target.id),this.dialog=!0},readIssue:function(){this.$router.push("/reader")}},watch:{issuesList:function(){var t=this;this.$nextTick(function(){t.pagination.totalItems=t.issuesList.length})}},computed:{pages:function(){return null==this.pagination.rowsPerPage||null==this.pagination.totalItems?0:Math.ceil(this.pagination.totalItems/this.pagination.rowsPerPage)},issuesList:function(){return this.$store.state.api.issueList},Issue:function(){return this.$store.state.api.selectedIssue[0]||{}},baseURL:function(){var t=this.$store.state.api.baseURL;return t}},mounted:function(){this.$store.state.test=this.$el.offsetHeight},data:function(){return{rowsPerPageItems:[5,10,15],pagination:{rowsPerPage:this.$store.state.api.gridSize},listtype:"Series",dialog:!1}}},u=c,v=(i("ec4b"),i("2877")),d=Object(v["a"])(u,e,a,!1,null,"1494522e",null);s["default"]=d.exports},b9b4:function(t,s,i){},ec4b:function(t,s,i){"use strict";var e=i("b9b4"),a=i.n(e);a.a}}]);
//# sourceMappingURL=chunk-6cf6f9b7.15562738.js.map