(function ($) {
    var row_id = 1;
    $.fn.orgChart = function (options) {
        var opts = $.extend({}, $.fn.orgChart.defaults, options);
        return new OrgChart($(this), opts);
    }

    $.fn.orgChart.defaults = {
        data: [{ UserGroupID: 1, GroupfullName: 'Root', ParentID: 0 }],
        showControls: false,
        allowEdit: false,
        onAddNode: null,
        onDeleteNode: null,
        onClickNode: null,
        newNodeText: '',
        EditNodeText: '',
       // DeleteNodeText: 'Delete Group'
    };

    function OrgChart($container, opts) {
        var data = opts.data;
        var nodes = {};
        var rootNodes = [];
        this.opts = opts;
        this.$container = $container;
        var self = this;

        this.draw = function () {
            $container.empty().append(rootNodes[0].render(opts));
            $container.find('.node').click(function () {
                if (self.opts.onClickNode !== null) {
                    self.opts.onClickNode(nodes[$(this).attr('node-id')]);
                }
            });

            if (opts.allowEdit) {
                $container.find('.node h2').click(function (e) {
                    var thisId = $(this).parent().attr('node-id');
                    self.startEdit(thisId);
                    e.stopPropagation();
                });
            }

            // add "add button" listener
            $("#AddNewGroupForm").submit(function (e) {
        
                var thisId = $(this).parent().attr('node-id');

                if (self.opts.onAddNode !== null) {
                    self.opts.onAddNode(nodes[thisId]);
                }
                else {
                    self.newNode(thisId);
                }
                e.stopPropagation();
            });

            $container.find('.org-del-button').click(function (e) {
                var thisId = $(this).parent().attr('node-id');

                if (self.opts.onDeleteNode !== null) {
                    self.opts.onDeleteNode(nodes[thisId]);
                }
                else {
                    self.deleteNode(thisId);
                }
                e.stopPropagation();
            });

        }

        this.startEdit = function (UserGroupID) {
            var inputElement = $('<input class="org-input" type="text" value="' + nodes[UserGroupID].data.GroupfullName + '"/>');
            $container.find('div[node-id=' + UserGroupID + '] h2').replaceWith(inputElement);
            var commitChange = function () {
                var h2Element = $('<h2>' + nodes[UserGroupID].data.GroupfullName + '</h2>');
                if (opts.allowEdit) {
                    h2Element.click(function () {
                        self.startEdit(UserGroupID);
                    })
                }
                inputElement.replaceWith(h2Element);
            }
            inputElement.focus();
            inputElement.keyup(function (event) {
                if (event.which == 13) {
                    commitChange();
                }
                else {
                    nodes[UserGroupID].data.GroupfullName = inputElement.val();
                }
            });
            inputElement.blur(function (event) {
                commitChange();
            })
        }

        this.newNode = function (ParentID) {
            var nextId = Object.keys(nodes).length;
            while (nextId in nodes) {
                nextId++;
            }

            self.addNode({ UserGroupID: nextId, GroupfullName: '', ParentID: ParentID });
        }

        this.addNode = function (data) {
            var newNode = new Node(data);
            nodes[data.UserGroupID] = newNode;
            nodes[data.ParentID].addChild(newNode);

            self.draw();
            self.startEdit(data.UserGroupID);
        }

        this.deleteNode = function () {
            for (var i = 0; i < nodes[UserGroupID].children.length; i++) {
                self.deleteNode(nodes[UserGroupID].children[i].data.UserGroupID);
            }
            nodes[nodes[UserGroupID].data.ParentID].removeChild(UserGroupID);
            delete nodes[UserGroupID];
            self.draw();
        }

        this.getData = function () {
            var outData = [];
            for (var i in nodes) {
                outData.push(nodes[i].data);
            }
            return outData;
        }

        // constructor
        for (var i in data) {
            var node = new Node(data[i]);
            nodes[data[i].UserGroupID] = node;
        }

        // generate parent child tree
        
        for (var i in nodes) {
            if (nodes[i].data.ParentID == 0) {
                rootNodes.push(nodes[i]);
            }
            else {
                nodes[nodes[i].data.ParentID].addChild(nodes[i]);

            }
        }

        // draw org chart
        $container.addClass('orgChart');
        self.draw();
    }

    function Node(data) {
        this.data = data;
        this.children = [];
        var self = this;

        this.addChild = function (childNode) {
            this.children.push(childNode);
        }

        this.removeChild = function (UserGroupID) {
            for (var i = 0; i < self.children.length; i++) {
                if (self.children[i].data.UserGroupID == UserGroupID) {
                    self.children.splice(i, 1);
                    return;
                }
            }
        }
        
        this.render = function (opts) {
            var childLength = self.children.length,
                mainTable;
            //var level = self.data.Level;
            
            mainTable = "<table class='table_parent " + row_id + "' cellpadding='0' cellspacing='0' border='0'>";
            var nodeColspan = childLength > 0 ? 2 * childLength : 2;
            

           
            mainTable += "<tr class='expanded " + row_id + "'><td colspan='" + nodeColspan + "'>" + self.formatNode(opts) + "";
            if (self.data.Level <= 2 && childLength > 0) {
                mainTable += "<div class='img_minus'><i class='fas fa-minus-circle'></i></td></tr>";
            }
            else if (self.data.Level > 2 && childLength > 0) {
                mainTable += "<div class='img_minus'><i class='fas fa-plus-circle'></i></td></tr>";
            }
            
            row_id++;
           
          
           

            if (childLength > 0) {
                var downLineTable = "<table cellpadding='0' cellspacing='0' border='0'><tr class='lines x'><td class='line left half'></td><td class='line right half'></td></table>";
                mainTable += "<tr class='lines'><td colspan='" + childLength * 2 + "'>" + downLineTable + '</td></tr>';
              
                
                var linesCols = '';
                for (var i = 0; i < childLength; i++) {
                    if (childLength == 1) {
                        linesCols += "<td class='line left half'></td>";    // keep vertical lines aligned if there's only 1 child
                    }
                    else if (i == 0) {
                        linesCols += "<td class='line left'></td>";     // the first cell doesn't have a line in the top
                    }
                    else {
                        linesCols += "<td class='line left top'></td>";
                    }

                    if (childLength == 1) {
                        linesCols += "<td class='line right half'></td>";
                    }
                    else if (i == childLength - 1) {
                        linesCols += "<td class='line right'></td>";
                    }
                    else {
                        linesCols += "<td class='line right top'></td>";
                    }
                }
                mainTable += "<tr class='lines v'>" + linesCols + "</tr>";
               

                mainTable += "<tr class='ligne " + self.data.Level + "'>";
                for (var i in self.children) {
                    mainTable += "<td colspan='2'>" + self.children[i].render(opts) + "</td>";
                }
                
                mainTable += "</tr>";
                
                
            }
           
            mainTable += '</table>';
            
            
            return mainTable;
            
            
        }
       
        this.formatNode = function (opts) {
            
            var nameString = '',
                descString = '';

            if (typeof data.GroupfullName !== 'undefined') {
                nameString = '<h2>' + self.data.GroupfullName + '</h2>';
            }

            if (typeof data.description !== 'undefined') {
                descString = '<p>' + self.data.description + '</p>';
            }
            if (opts.showControls) {
               
                if ((this.data.ParentID != 0 || this.data.UserGroupID == this.data.AdminGroupID) && this.data.IsBrand == false) {
                
                    //<div class=\"org-del-button\" id=\"delete-group\" onclick=\"editItem(" + this.data.UserGroupID + ")\">" + opts.DeleteNodeText + " </div>
                    var buttonsHtml = "<div class=\"org-add-button\" id=\"add-group\" onclick=\"OpenAddGroupWindow(" + this.data.UserGroupID + ",'" + this.data.GroupfullName + "')\"><i class=\"fas fa-plus\"></i></div><div onclick=\"editItem(" + this.data.UserGroupID + ")\" class=\"org-edit-button\" id=\"edit-group\"><i class=\"fas fa-pencil-alt\"></i></div>";
                }

                else if (this.data.IsBrand &&  this.data.AnchorGroupID != this.data.UserGroupID  ) {

                    var buttonsHtml = "<div class=\"org-add-button\" id=\"add-group\" onclick=\"OpenAddGroupWindow(" + this.data.UserGroupID + ",'" + this.data.GroupfullName + "')\"><i class=\"fas fa-plus\"></i></div><div class=\"brand-button\" id=\"brand\"><i class=\"fa fa-star\"></i></div><div onclick=\"editItem(" + this.data.UserGroupID + ")\" class=\"org-edit-button\" id=\"edit-group\"><i class=\"fas fa-pencil-alt\"></i></div>";

                }

                else if (this.data.AnchorGroupID == this.data.UserGroupID && this.data.IsBrand) {
                    
                    var buttonsHtml = "<div class=\"org-add-button\" id=\"add-group\" onclick=\"OpenAddGroupWindow(" + this.data.UserGroupID + ",'" + this.data.GroupfullName + "')\"><i class=\"fas fa-plus\"></i></div><div class=\"brand-button\" id=\"brand\"><i class=\"fa fa-star\"></i></div>";
                }
                else if (this.data.AnchorGroupID == this.data.UserGroupID && !(this.data.IsBrand)) {

                    var buttonsHtml = "<div class=\"org-add-button\" id=\"add-group\" onclick=\"OpenAddGroupWindow(" + this.data.UserGroupID + ",'" + this.data.GroupfullName + "')\"></div>";
                }
            }

            else {
                buttonsHtml = "";
            }


            return "<div class='node' node-id='" + this.data.UserGroupID + "'>" + descString + buttonsHtml + nameString + "</div>";
        }
    }

})(jQuery);

