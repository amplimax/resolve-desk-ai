import{Aa as c,Bb as A,Cb as D,Db as O,Gb as R,I as b,K as l,La as x,Ma as p,Na as _,Pa as w,Qa as y,R as f,V as g,Wa as M,Xa as C,Ya as r,_ as u,hc as E,ia as s,ja as h,jc as I,ka as v,kc as S,ub as k,xb as T,ya as n,za as a,zb as P}from"./chunk-U22BCNOZ.js";var z=[{path:"",pathMatch:"full",redirectTo:"dashboard"},{path:"dashboard",title:"Overview | ResolveDesk AI",loadComponent:()=>import("./chunk-UUBK36HW.js").then(t=>t.DashboardPage)},{path:"tickets/new",loadComponent:()=>import("./chunk-KTTO37DU.js").then(t=>t.TicketCreatePage)},{path:"tickets/:id/edit",loadComponent:()=>import("./chunk-WMMWSWL7.js").then(t=>t.TicketEditPage)},{path:"tickets/:id",loadComponent:()=>import("./chunk-TVHNDU3Y.js").then(t=>t.TicketDetailPage)},{path:"tickets",title:"Tickets | ResolveDesk AI",loadComponent:()=>import("./chunk-EVI7QYY3.js").then(t=>t.TicketsPage)},{path:"**",redirectTo:"dashboard"}];var L={providers:[g(),T(),O(z)]};var U=["*",[["mat-toolbar-row"]]],V=["*","mat-toolbar-row"],H=(()=>{class t{static \u0275fac=function(e){return new(e||t)};static \u0275dir=v({type:t,selectors:[["mat-toolbar-row"]],hostAttrs:[1,"mat-toolbar-row"],exportAs:["matToolbarRow"]})}return t})(),B=(()=>{class t{_elementRef=l(u);_platform=l(R);_document=l(f);color;_toolbarRows;constructor(){}ngAfterViewInit(){this._platform.isBrowser&&(this._checkToolbarMixedModes(),this._toolbarRows.changes.subscribe(()=>this._checkToolbarMixedModes()))}_checkToolbarMixedModes(){this._toolbarRows.length}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=s({type:t,selectors:[["mat-toolbar"]],contentQueries:function(e,i,F){if(e&1&&_(F,H,5),e&2){let d;w(d=y())&&(i._toolbarRows=d)}},hostAttrs:[1,"mat-toolbar"],hostVars:6,hostBindings:function(e,i){e&2&&(C(i.color?"mat-"+i.color:""),M("mat-toolbar-multiple-rows",i._toolbarRows.length>0)("mat-toolbar-single-row",i._toolbarRows.length===0))},inputs:{color:"color"},exportAs:["matToolbar"],ngContentSelectors:V,decls:2,vars:0,template:function(e,i){e&1&&(x(U),p(0),p(1,1))},styles:[`.mat-toolbar {
  background: var(--mat-toolbar-container-background-color, var(--mat-sys-surface));
  color: var(--mat-toolbar-container-text-color, var(--mat-sys-on-surface));
}
.mat-toolbar, .mat-toolbar h1, .mat-toolbar h2, .mat-toolbar h3, .mat-toolbar h4, .mat-toolbar h5, .mat-toolbar h6 {
  font-family: var(--mat-toolbar-title-text-font, var(--mat-sys-title-large-font));
  font-size: var(--mat-toolbar-title-text-size, var(--mat-sys-title-large-size));
  line-height: var(--mat-toolbar-title-text-line-height, var(--mat-sys-title-large-line-height));
  font-weight: var(--mat-toolbar-title-text-weight, var(--mat-sys-title-large-weight));
  letter-spacing: var(--mat-toolbar-title-text-tracking, var(--mat-sys-title-large-tracking));
  margin: 0;
}
@media (forced-colors: active) {
  .mat-toolbar {
    outline: solid 1px;
  }
}
.mat-toolbar .mat-form-field-underline,
.mat-toolbar .mat-form-field-ripple,
.mat-toolbar .mat-focused .mat-form-field-ripple {
  background-color: currentColor;
}
.mat-toolbar .mat-form-field-label,
.mat-toolbar .mat-focused .mat-form-field-label,
.mat-toolbar .mat-select-value,
.mat-toolbar .mat-select-arrow,
.mat-toolbar .mat-form-field.mat-focused .mat-select-arrow {
  color: inherit;
}
.mat-toolbar .mat-input-element {
  caret-color: currentColor;
}
.mat-toolbar .mat-mdc-button-base.mat-mdc-button-base.mat-unthemed {
  --mat-button-text-label-text-color: var(--mat-toolbar-container-text-color, var(--mat-sys-on-surface));
  --mat-button-outlined-label-text-color: var(--mat-toolbar-container-text-color, var(--mat-sys-on-surface));
}

.mat-toolbar-row, .mat-toolbar-single-row {
  display: flex;
  box-sizing: border-box;
  padding: 0 16px;
  width: 100%;
  flex-direction: row;
  align-items: center;
  white-space: nowrap;
  height: var(--mat-toolbar-standard-height, 64px);
}
@media (max-width: 599px) {
  .mat-toolbar-row, .mat-toolbar-single-row {
    height: var(--mat-toolbar-mobile-height, 56px);
  }
}

.mat-toolbar-multiple-rows {
  display: flex;
  box-sizing: border-box;
  flex-direction: column;
  width: 100%;
  min-height: var(--mat-toolbar-standard-height, 64px);
}
@media (max-width: 599px) {
  .mat-toolbar-multiple-rows {
    min-height: var(--mat-toolbar-mobile-height, 56px);
  }
}
`],encapsulation:2,changeDetection:0})}return t})();var N=(()=>{class t{static \u0275fac=function(e){return new(e||t)};static \u0275mod=h({type:t});static \u0275inj=b({imports:[E]})}return t})();var m=class t{static \u0275fac=function(o){return new(o||t)};static \u0275cmp=s({type:t,selectors:[["app-root"]],decls:23,vars:0,consts:[[1,"app-shell"],[1,"topbar"],["routerLink","/dashboard","aria-label","ResolveDesk AI dashboard",1,"brand"],["aria-hidden","true",1,"brand__mark"],[1,"brand__text"],[1,"brand__name"],[1,"brand__tagline"],[1,"topbar__spacer"],["aria-label","Primary navigation",1,"navigation"],["mat-button","","routerLink","/dashboard","routerLinkActive","navigation__link--active"],["mat-button","","routerLink","/tickets","routerLinkActive","navigation__link--active"],[1,"content"],[1,"footer"]],template:function(o,e){o&1&&(n(0,"div",0)(1,"mat-toolbar",1)(2,"a",2)(3,"span",3),r(4,"R"),a(),n(5,"span",4)(6,"span",5),r(7,"ResolveDesk AI"),a(),n(8,"span",6),r(9,"Intelligent support operations"),a()()(),c(10,"span",7),n(11,"nav",8)(12,"a",9),r(13," Overview "),a(),n(14,"a",10),r(15," Tickets "),a()()(),n(16,"main",11),c(17,"router-outlet"),a(),n(18,"footer",12)(19,"span"),r(20,"ResolveDesk AI"),a(),n(21,"span"),r(22,"ASP.NET Core \xB7 Angular \xB7 Applied AI"),a()()())},dependencies:[A,D,P,S,I,N,B],styles:["[_nghost-%COMP%]{display:block;min-height:100dvh}.app-shell[_ngcontent-%COMP%]{display:flex;min-height:100dvh;flex-direction:column}.topbar[_ngcontent-%COMP%]{position:sticky;z-index:10;top:0;min-height:72px;padding-inline:clamp(16px,4vw,48px);border-bottom:1px solid var(--mat-sys-outline-variant);background:color-mix(in srgb,var(--mat-sys-surface) 94%,transparent);-webkit-backdrop-filter:blur(16px);backdrop-filter:blur(16px)}.brand[_ngcontent-%COMP%]{display:inline-flex;align-items:center;gap:12px;color:inherit;text-decoration:none}.brand__mark[_ngcontent-%COMP%]{display:grid;width:42px;height:42px;place-items:center;border-radius:13px;background:var(--mat-sys-primary);color:var(--mat-sys-on-primary);font-size:1.25rem;font-weight:700}.brand__text[_ngcontent-%COMP%]{display:flex;flex-direction:column;line-height:1.2}.brand__name[_ngcontent-%COMP%]{font-size:1rem;font-weight:700;letter-spacing:.01em}.brand__tagline[_ngcontent-%COMP%]{color:var(--mat-sys-on-surface-variant);font-size:.75rem;font-weight:400}.topbar__spacer[_ngcontent-%COMP%]{flex:1}.navigation[_ngcontent-%COMP%]{display:flex;gap:4px}.navigation__link--active[_ngcontent-%COMP%]{background:var(--mat-sys-secondary-container);color:var(--mat-sys-on-secondary-container)}.content[_ngcontent-%COMP%]{width:min(1180px,100% - 32px);flex:1;margin-inline:auto;padding-block:clamp(32px,5vw,64px)}.footer[_ngcontent-%COMP%]{display:flex;justify-content:space-between;gap:16px;padding:20px clamp(16px,4vw,48px);border-top:1px solid var(--mat-sys-outline-variant);color:var(--mat-sys-on-surface-variant);font-size:.8rem}@media(max-width:640px){.brand__tagline[_ngcontent-%COMP%]{display:none}.topbar[_ngcontent-%COMP%]{padding-inline:12px}.navigation[_ngcontent-%COMP%]   a[_ngcontent-%COMP%]{min-width:auto;padding-inline:10px}.footer[_ngcontent-%COMP%]{flex-direction:column}}"]})};k(m,L).catch(t=>console.error(t));
