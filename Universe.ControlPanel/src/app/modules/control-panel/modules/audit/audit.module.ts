import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { TableModule } from "primeng/table";
import { TabViewModule } from "primeng/tabview";

import { GridModule } from "@features/grid";
import { DataAuditService, SignInAuditService } from "./services";
import { CorePermissions } from "../../static-data";
import { RightsRequirementGuard } from "../../../../guards";
import { AuditComponent, DataAuditComponent, SignInAuditComponent } from "./components";


@NgModule({
	declarations: [
		AuditComponent,
		DataAuditComponent,
		SignInAuditComponent
	],
	imports: [
		GridModule,

		TabViewModule,
		TableModule,

		RouterModule.forChild([
			{
				path: "",
				component: AuditComponent,
				data: { permissions: CorePermissions.ReadAuditData },
				canActivate: [ RightsRequirementGuard ],
			}
		])
	],
	providers: [ DataAuditService, SignInAuditService ]
})
export class AuditModule {}