Vue.component('teacher-profiles', {
    template: '#teacher-profiles-component',
    data: function () {
        return {
            rows: [],
            endpoint: ""
        };
    },
    mounted: function() {
        this.fetchInitial();
    },
    methods: {
        fetchInitial: function() {
            var initialView = JSON.parse(this.$el.dataset.initial);

            this.updateTeachers(initialView);
        },
        fetchNextPage: function() {
            fetch(this.endpoint)
                .then(function(response) {
                    return response.json();
                })
                .then(this.updateTeachers);
        },
        updateTeachers: function(response) {
            this.endpoint = response.nextPageEndpoint;
            this.rows.push(response.teachers);
        }
    }
});

var teacherProfiles = new Vue({
    el: '#teacher-profiles-app'
});